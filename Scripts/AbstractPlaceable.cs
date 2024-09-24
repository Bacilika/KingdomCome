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
	protected PlaceableInfo InfoBox;
	protected int Price;
	protected int Workers =0;
	public int Level;
	protected int WoodCost;
	protected int StoneCost;
	private int _maxLevel = 2;
	protected int Citizens;
	private CollisionShape2D _hitbox;
	public Dictionary<string, List<int>> Upgrades; 
	protected AnimatedSprite2D AnimatedSprite;
	private double _time;
	private bool _move;

	[Signal]
	public delegate void OnMoveBuildingEventHandler(AbstractPlaceable building);
	
	[Signal]
	public delegate void OnAreaUpdatedEventHandler(bool status);
	[Signal]
	public delegate void OnBuildingUpgradeEventHandler(AbstractPlaceable building);
	
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		AnimatedSprite = GetNode<AnimatedSprite2D>("HouseSprite");
		
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
		InfoBox.Connect(PlaceableInfo.SignalName.OnMove, Callable.From(OnMove));
		Monitoring = true;
		Monitorable = true;
		InfoBox.Visible = false;
		InfoBox.MoveToFront();
		_Ready_instance();
		SetObjectValues();
		
	}

	public override void _Process(double delta)
	{
		if (IsPlaced)
		{
			_time += delta;
			if (_time > 1)
			{
				_time -= 1;
				Tick();
			}
			InfoBox.MoveToFront();
		}
	}

	protected abstract void Tick();

	public abstract void _Ready_instance();
	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			_isFocused = true;
		}			
	}
	
	public void OnMouseExited()
	{
		_isFocused = false;
	}
	
	public void OnAreaEntered(Area2D other)
	{
		
		if(IsPlaced)
		{
			GD.Print("Area entered");
			EmitSignal(SignalName.OnAreaUpdated,true);
		}			
	}
	
	public void OnAreaExited(Area2D other)
	{
		GD.Print("area exited");
		EmitSignal(SignalName.OnAreaUpdated,false);
	}
	
	public int GetPrice()
	{
		return Price;
	}
	

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Inputs.LeftClick) && !GameMenu.IsPlaceMode)
		{
			if (_isFocused) //if mouse is on Building
			{
				InfoBox.Visible = !InfoBox.Visible;
			}
			else //if building is not focused
			{
				if (!InfoBox.Focused) //and infobox is not focused
				{
					InfoBox.Visible = false;
				}
				else
				{
					InfoBox.Focused = true;
				}
			}
		}
	}
	protected abstract void OnDelete();

	protected async void OnUpgrade()
	{
		if (Level <_maxLevel)
		{
			if (await EnoughSpace())
			{
				GD.Print("No collision");
				ActivateHitbox(Level); //return to old hitbox
				Level++;
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
	public void OnMove()
	{
		InfoBox.Visible = false;
		IsPlaced = false;
		EmitSignal(SignalName.OnMoveBuilding, this);

	}

	private void ActivateHitbox(int level)
	{
		GD.Print("Activating hitbox for level: " + level);
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
	
	public void SetObjectValues()
	{
		AnimatedSprite.Frame = Level;
		Price = Upgrades["Cost"][Level];
		ActivateHitbox(Level);
	}

	private async Task<bool> EnoughSpace()
	{
		ActivateHitbox(Level+1); //try with larger hitbox
		await Task.Delay(100);
		GD.Print( GetOverlappingAreas().Count);
		return !HasOverlappingAreas();

	}
}
