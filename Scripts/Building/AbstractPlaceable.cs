using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Scripts.Constants;

namespace KingdomCome.Scripts.Building;

public abstract partial class AbstractPlaceable : Area2D
{
    [Signal]
    public delegate void OnAreaUpdatedEventHandler(bool status);

    [Signal]
    public delegate void OnBuildingUpgradeEventHandler(AbstractPlaceable building);

    [Signal]
    public delegate void OnMoveBuildingEventHandler(AbstractPlaceable building);

    private CollisionShape2D _hitbox;
    private bool _isFocused;
    private int _maxLevel = 2;
    private Color _modulation = new(1, 0, 0);
    private bool _move;
    private Color _noModulation = new(1, 1, 1);

    private double _time;
    private ChooseWare _wareBox;
    public AnimatedSprite2D HouseSprite;
    public Dictionary<string, List<int>> BuildCost;
    public string BuildingDescription;

    public string BuildingName;
    public bool Colliding;
    public Dictionary<string, List<int>> DeleteCost;
    protected int HouseholdHappiness;

    public PlaceableInfo InfoBox;
    protected int Inhabitants;
    public bool IsPlaced;
    public bool IsUnlocked = false;

    public int Level;
    public Dictionary<string, List<int>> MoveCost;
    public List<Npc> People = [];
    public int PlayerLevel = 0;
    protected List<Npc> CurrentPeople = [];

    public bool ActivityIndoors = true;

    protected RandomNumberGenerator Rnd = new();

    //Used by workbench
    public int BuildingCounter = 0;
    public bool isDone = false; 


    public Dictionary<string, List<int>> Upgrades;

    protected virtual void Tick()
    {
        
    }
    protected abstract void _Ready_instance();

    protected virtual void OnDeleteInstance()
    {
    }

    public virtual void OnParentReady()
    {
    }

    //Only for Living spaces
    protected virtual void OnDelete()
    {
        OnDeleteInstance();
        for (var i = People.Count - 1; i >= 0; i--)
        {
            var npc = People[i];
            npc.OnDelete();
        }
        foreach (var cost in DeleteCost) GameLogistics.Resources[cost.Key] += cost.Value[Level];
        Shop.deleteAudio.Play();
        
        QueueFree();
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ZIndex = 2; //in front of Npc
        InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
        HouseSprite = GetNode<AnimatedSprite2D>("HouseSprite");

        InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
        InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
        InfoBox.Connect(PlaceableInfo.SignalName.OnMove, Callable.From(OnMove));

        Monitoring = true;
        Monitorable = true;
        InfoBox.Visible = false;
        InfoBox.MoveToFront();
        _wareBox = InfoBox.GetNode<ChooseWare>("ChooseWare");
        _wareBox.Visible = false;
        var button = InfoBox.GetNode<Button>("ChooseWareButton");
        button.Visible = this is MarketStall;
        BodyEntered += CitizenEntered;
        BodyExited += CitizenExited;
        if(!HouseSprite.SpriteFrames.GetAnimationNames().Contains("Building"))
            HouseSprite.SpriteFrames.AddAnimation("Building");
        HouseSprite.SpriteFrames.AddFrame("Building",(Texture2D)GD.Load("res://Sprites/Extra/Building.png/"));
        
 

        _Ready_instance();
        SetObjectValues();
        OnParentReady();
    }
    
    public  virtual void CitizenEntered(Node2D node2D)
    {

        if (node2D is Npc npc)
        {
            CurrentPeople.Add(npc);
            npc.OnBuildingEntered(this);
        }
    }
    public void CitizenExited(Node2D node2D)
    {

        if (node2D is Npc npc)
        {
            CurrentPeople.Remove(npc);
            npc.OnBuildingExited();
        }
			
    }

    public string GetBuildingName()
    {
        return GetType().Name;
    }

    public override void _Process(double delta)
    {
        if (IsPlaced && isDone)
        {
            HouseSprite.SelfModulate = Colliding ? _modulation : _noModulation;

            _time += delta;
            if (_time > 1)
            {
                _time -= 1;
                Tick();
            }

            InfoBox.MoveToFront();
        }
    }

    private void OnMouseEntered()
    {
        if (IsPlaced) _isFocused = true;
    }

    private void OnMouseExited()
    {
        _isFocused = false;
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is Tree tree)
        {
            Console.WriteLine("Tree collision");
        }
        else
        {
            var building = (AbstractPlaceable)other;
            building.Colliding = true;
            if (IsPlaced) EmitSignal(SignalName.OnAreaUpdated, true);
        }
    }

    private void OnAreaExited(Area2D other)
    {
        if (other is Tree tree)
        {
            Console.WriteLine("Tree exited");
        }
        else
        {
            var building = (AbstractPlaceable)other;
            building.Colliding = false;
            EmitSignal(SignalName.OnAreaUpdated, false);
        }
    }


    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(Inputs.LeftClick) && !GameLogistics.IsPlaceMode)
        {
            if (_isFocused) //if mouse is on Building
            {
                if (GameMap.JobSelectMode && this is Production production)
                {
                    var employed = production.EmployWorker(GameMap.NpcJobSelect);
                    if (employed)
                    {
                        GameMap.NpcJobSelect.Work = production;
                        GameMap.JobSelectMode = false;
                        InfoBox.HideNpcInfo();
                        GameMenu.GameMode.Text = "";
                    }
                }
                else
                {
                    InfoBox.Visible = !InfoBox.Visible;
                    InfoBox.HideNpcInfo();
                    InfoBox.MoveToFront();
                }
            }
            else //if building is not focused
            {
                if (!InfoBox.Focused && !_wareBox.Focused) //and infobox is not focused
                    InfoBox.Visible = false;
                else
                    InfoBox.Focused = true;
            }
        }

        if (@event.IsActionPressed(Inputs.RightClick)) GameMap.JobSelectMode = false;
    }

    public string CostToString()
    {
        var result = "";
        foreach (var cost in BuildCost) result += $"{cost.Key}: {cost.Value[Level]}\n";
        return result;
    }

    private async void OnUpgrade()
    {
        if (Level < _maxLevel)
        {
            if (await EnoughSpace())
            {
                GD.Print("No collision");
                ActivateHitbox(Level); //return to old hitbox
                Level++;
                if (this is House)
                {
                    HouseSprite.SetAnimation("Level" + Level);
                    HouseSprite.Pause();
                }

                EmitSignal(SignalName.OnBuildingUpgrade, this);

                SetObjectValues();
                Shop.placeAudio.Play();
            }
            else
            {
                GD.Print("Collision when trying to upgrade");
                ActivateHitbox(Level); //return to old hitbox
            }
        }
    }

    private void OnMove()
    {
        InfoBox.Visible = false;
        IsPlaced = false;
        EmitSignal(SignalName.OnMoveBuilding, this);
    }

    private void ActivateHitbox(int level)
    {
        foreach (var child in GetChildren())
            if (child is CollisionShape2D shape2D)
            {
                if (shape2D.Name == "CollisionShape" + level)
                {
                    shape2D.Disabled = false;
                    _hitbox = shape2D;
                }
                else
                {
                    shape2D.Disabled = true;
                }

                shape2D.Visible = true;
            }
    }

    public void SetObjectValues()
    {
        HouseSprite.SetAnimation("Level" + Level); 
        ActivateHitbox(Level);
    }

    private async Task<bool> EnoughSpace()
    {
        ActivateHitbox(Level + 1); //try with larger hitbox
        await Task.Delay(100);
        GD.Print(GetOverlappingAreas().Count);
        return !HasOverlappingAreas();
    }

    protected void PlayAnimation()
    {
        var animatedSprite = GetNode<AnimatedSprite2D>("Animation");
        animatedSprite?.Play();
    }
    protected void PlayAnimation(Vector2 position)
    {
        var animatedSprite = GetNode<AnimatedSprite2D>("Animation");
        animatedSprite.MoveToFront();
        animatedSprite.GlobalPosition = position;
        animatedSprite?.Play();
    }
}