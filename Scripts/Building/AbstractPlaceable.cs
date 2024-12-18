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
    public delegate void OnMoveBonupguildingEventHandler(AbstractPlaceable building);
    
    public static Dictionary<AbstractPlaceable, List<Npc>> BuildList = [];


    public CollisionShape2D _hitbox;
    private bool _isFocused;
    private int _maxLevel = 2;
    private Color _modulation = new(1, 0, 0);
    private bool _move;
    private Color _noModulation = new(1, 1, 1);

    private double _time;
    public AnimatedSprite2D HouseSprite;
    public Dictionary<string, List<int>> BuildCost;
    public string BuildingDescription;

    public string BuildingName;
    public bool Colliding;
    public Dictionary<string, List<int>> DeleteCost;
    protected int HouseholdHappiness;

    public PlaceableInfo InfoBox;
    public int Inhabitants;
    public bool IsPlaced;
    public bool IsUnlocked = false;

    public int Level;
    public Dictionary<string, List<int>> MoveCost;
    public List<Npc> People = [];
    public int PlayerLevel = 0;
    public List<Npc> CurrentPeople = []; // npcs who are currently at the location

    public bool ActivityIndoors = true;

    protected RandomNumberGenerator Rnd = new();
    public Dictionary<string, List<int>> Upgrades;


    //Used by workbench
    public int BuildingCounter = 0;
    public bool isDone = false;
    public bool isUpgrading = false;
    
    //Progress bar
    public ProgressBar BuildingProgressBar = new();
    public ProgressBar Upgradeprogressbar = new();

    protected AnimatedSprite2D _animatedSprite;


    //Virtual and abstract funtcions
    protected virtual void Tick(){}
    protected abstract void _Ready_instance();
    protected virtual void OnDeleteInstance(){}
    public virtual void OnParentReady(){}
    
    

    //Only for Living spaces
    protected virtual void OnDelete()
    {
        OnDeleteInstance();
        for (var i = People.Count - 1; i >= 0; i--)
        {
            var npc = People[i];
            // TODO: dont delte npc, remove them from house
            npc.OnHouseDestroyed();
        }
        foreach (var cost in DeleteCost) GameLogistics.Resources[cost.Key] += cost.Value[Level];
        Shop.deleteAudio.Play();
        foreach (var house in GameMap._placedHouses)
        {
            if (house == this)
            {
                GameMap._placedHouses.Remove(house);
                break;
            }
        }
        foreach (var activity in GameMap._placedActivities)
        {
            if (activity == this)
            {
                GameMap._placedActivities.Remove(activity);
                break;
            }
        }
        QueueFree();
    }

    protected virtual void TurnOffBuilding()
    {
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
        
        var turnoffbutton = InfoBox.GetNode<Button>("HouseInfo/ButtonContainer/TurnOffButton");
        turnoffbutton.Visible = this is Production;
        BodyEntered += CitizenEntered;
        BodyExited += CitizenExited;
        if(!HouseSprite.SpriteFrames.GetAnimationNames().Contains("Building"))
            HouseSprite.SpriteFrames.AddAnimation("Building");
        HouseSprite.SpriteFrames.AddFrame("Building",(Texture2D)GD.Load("res://Sprites/Extra/Building.png/"));
        
 		InfoBox.Connect(PlaceableInfo.SignalName.OnTurnOffBuilding, Callable.From(TurnOffBuilding));

        //Remove buttons from decorations
        InfoBox.GetNode<Button>("HouseInfo/ButtonContainer/UpgradeButton").Visible = this is not Decoration.Decoration;
        
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

        if (InfoBox.Visible)
        {
            InfoBox.DeleteButton.TooltipText ="Resources Back On Delete\n" +DeleteResources();
            InfoBox.UpgradeButton.TooltipText ="Cost To Upgrade\n" + UpgradeResources();
        }
    }

    private string DeleteResources()
    {
        var result = "";
        foreach (var resource in DeleteCost)
        {
            result += $"{resource.Key}: {resource.Value[Level]}";
        }

        return result;
    }
    private string UpgradeResources()
    {
        var result = "";
        if (Level == 2) return "Max Level Reached";
        foreach (var resource in BuildCost)
        {
            
            result += $"{resource.Key}: {resource.Value[Level+1]}";
        }

        return result;
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


    public virtual void OnBuildingPressed() { }
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
                        GameMenu.GameMode.Text = "";
                    }
                }
                else
                {
                    OnBuildingPressed();
                    InfoBox.Visible = !InfoBox.Visible;
                    InfoBox.HideNpcInfo();
                    InfoBox.MoveToFront();
                }
            }
            else //if building is not focused
            {
                if (!InfoBox.Focused) //and infobox is not focused
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
        BuildingCounter = 0;
        if (Level < _maxLevel)
        {
            if (await EnoughSpace())
            {
                isUpgrading = true;
                isDone = false;
                HouseSprite.SetAnimation("Building"); 
                Level++;
                BuildList.Add(this, []);
                ActivateHitbox(Level); 
                EmitSignal(SignalName.OnBuildingUpgrade, this);
                Shop.placeAudio.Play();
                
                if (this is not WorkBench)
                {
                    AddChild(Upgradeprogressbar);
                    Upgradeprogressbar.MinValue = 0;
                    Upgradeprogressbar.GlobalPosition = GlobalPosition + new Vector2(-50, -60);
                    Upgradeprogressbar.MaxValue = 25;
                    Upgradeprogressbar.Visible = true;
                    Upgradeprogressbar.ZIndex = 1;
                    Upgradeprogressbar.ShowPercentage = false;
                    var theme = GD.Load<Theme>("res://Themes/Theme.tres");
                    Upgradeprogressbar.Theme = theme;
                    Upgradeprogressbar.Size = new Vector2(BuildingProgressBar.Size.X, 15); 
                    Upgradeprogressbar.SetCustomMinimumSize(new Vector2(100, 30));
                }

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
    protected void PlayAnimation(Vector2 position)
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("Animation");
        _animatedSprite.Animation = "default";
        _animatedSprite.MoveToFront();
        _animatedSprite.GlobalPosition = position;
        _animatedSprite?.Play();
    }protected void PlayAnimation()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("Animation");
        _animatedSprite.Animation = "default";
        _animatedSprite.MoveToFront();
        _animatedSprite?.Play();
    }
}