using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Area2D
{
	public bool IsPlaced;
	private bool _isFocused;
	private bool _move;
	public bool isUnlocked = false;
	public bool hasMoved;
	
	public int Level;
	private int _maxLevel = 2;
	protected int Inhabitants;
	protected int HouseholdHappiness;
	public int PlayerLevel = 0;
	
	private double _time;
	
	public string BuildingName;
	public string BuildingDescription;
	
	public PlaceableInfo InfoBox;
	public AnimatedSprite2D AnimatedSprite;
	private CollisionShape2D _hitbox;
	public ChooseWare WareBox;
	public bool Colliding;
	private Color _noModulation = new (1, 1, 1);
	private Color _modulation = new (1, 0, 0);
	
	
	public Dictionary<string, List<int>> Upgrades;
	public Dictionary<string, List<int>> BuildCost;
	public Dictionary<string, List<int>> MoveCost;
	public Dictionary<string, List<int>> DeleteCost;
	public List<Npc> People = [];
	
	protected RandomNumberGenerator Rnd = new ();
	
	[Signal]
	public delegate void OnMoveBuildingEventHandler(AbstractPlaceable building);
	[Signal]
	public delegate void OnAreaUpdatedEventHandler(bool status);
	[Signal]
	public delegate void OnBuildingUpgradeEventHandler(AbstractPlaceable building);
	
	protected abstract void Tick();
	public abstract void _Ready_instance();
	protected abstract void OnDeleteInstance();
	public void OnDelete()
	{
		OnDeleteInstance();
		for (int i = People.Count-1; i > 0; i--)
		{
			var npc = People[i];
			npc.OnDelete();
		}
		
		foreach (var cost in DeleteCost)
		{
			GameLogistics.Resources[cost.Key] -= cost.Value[Level];
		}
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ZIndex = 2; //in front of Npc
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		AnimatedSprite = GetNode<AnimatedSprite2D>("HouseSprite");
		
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
		InfoBox.Connect(PlaceableInfo.SignalName.OnMove, Callable.From(OnMove));
		
		Monitoring = true;
		Monitorable = true;
		InfoBox.Visible = false;
		InfoBox.MoveToFront();
		WareBox	= InfoBox.GetNode<ChooseWare>("ChooseWare");
		WareBox.Visible = false;
		var _button = InfoBox.GetNode<Button>("ChooseWareButton");
		if (this is MarketStall)
		{
			_button.Visible = true;
		}
		else
		{
			_button.Visible = false;
		}
		
		_Ready_instance();
		SetObjectValues();
	}

	public string GetBuildingName()
	{
		return GetType().Name;
	}

	public override void _Process(double delta)
	{
		if (IsPlaced)
		{
			if (Colliding)
			{
				AnimatedSprite.SelfModulate = _modulation;
			}
			else
			{
				AnimatedSprite.SelfModulate =_noModulation;
			}
			
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

		if(IsPlaced)
		{
			_isFocused = true;
		}			
	}
	
	private void OnMouseExited()
	{
		_isFocused = false;
	}
	
	private void OnAreaEntered(Area2D other)
	{
		var building = (AbstractPlaceable)other;
		building.Colliding = true;
		if(IsPlaced)
		{
			EmitSignal(SignalName.OnAreaUpdated,true);
		}			
	}
	
	private void OnAreaExited(Area2D other)
	{
		var building = (AbstractPlaceable)other;
		building.Colliding = false;
		EmitSignal(SignalName.OnAreaUpdated,false);
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
				if (!InfoBox.Focused && !WareBox.Focused) //and infobox is not focused
				{
					InfoBox.Visible = false;
				}
				else
				{
					InfoBox.Focused = true;
				}
			}
		}

		if (@event.IsActionPressed(Inputs.RightClick))
		{
			GameMap.JobSelectMode = false;
		}
	}

	public string CostToString()
	{
		string result = "";
		foreach (var cost in BuildCost)
		{
			result += $"{cost.Key}: {cost.Value[Level]}\n";
		}
		return result;
	}

	private async void OnUpgrade()
	{
		if (Level <_maxLevel)
		{
			if (await EnoughSpace())
			{
				GD.Print("No collision");
				ActivateHitbox(Level); //return to old hitbox
				Level++;
				if (this is House)
				{
					GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("default");
					GetNode<AnimatedSprite2D>("HouseSprite").Pause();
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
		hasMoved = true;
	}

	private void ActivateHitbox(int level)
	{
		foreach (var child in GetChildren())
		{
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
	}

	public abstract void WhenShopReady();
	
	public void SetObjectValues()
	{
		AnimatedSprite.Frame = Level;
		ActivateHitbox(Level);
	}

	private async Task<bool> EnoughSpace()
	{
		ActivateHitbox(Level+1); //try with larger hitbox
		await Task.Delay(100);
		GD.Print( GetOverlappingAreas().Count);
		return !HasOverlappingAreas();
	}
	public void PlayAnimation()
	{
		var animatedSprite = GetNode<AnimatedSprite2D>("Animation");

		if (animatedSprite is not null)
		{
			animatedSprite.Play();
		}

	}
}
