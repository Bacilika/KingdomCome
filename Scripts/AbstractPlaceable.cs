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
	protected int Workers;
	protected int Level;
	private int _maxLevel = 2;
	protected int Citizens;
	private CollisionShape2D _hitbox;
	protected Dictionary<string, List<int>> Upgrades; 
	protected AnimatedSprite2D AnimatedSprite;
	private double _time;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		AnimatedSprite = GetNode<AnimatedSprite2D>("HouseSprite");
		
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
		Monitoring = true;
		Monitorable = true;
		InfoBox.Visible = false;
		InfoBox.MoveToFront();
		_Ready_instance();
		SetObjectValues();
		
	}

	public override void _Process(double delta)
	{
		if (!IsPlaced) return;

		_time += delta;
		if (_time > 1)
		{
			_time -= 1;
			Tick();
			
		}
		InfoBox.MoveToFront();
		
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
			GameMenu.ContainHouse = true;
		}			
	}
	
	public void OnAreaExited(Area2D other)
	{
		GameMenu.ContainHouse = false;
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

	protected void OnUpgrade()
	{
		if (Level <_maxLevel && Upgrades["Cost"][Level] < GameMenu.Money)
		{
			EnoughSpace();

		}
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

	protected void SetObjectValues()
	{
		AnimatedSprite.Frame = Level;
		Price = Upgrades["Cost"][Level];
		ActivateHitbox(Level);
	}

	private async void EnoughSpace()
	{
		ActivateHitbox(Level+1); //try with larger hitbox
		_= await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		GD.Print( GetOverlappingAreas().Count);
		if ( GetOverlappingAreas().Count > 0)
		{ GD.Print("Collision when trying to upgrade");
			ActivateHitbox(Level); //return to old hitbox
		}
		else
		{
			GD.Print("No collision, upgrading house");
			Level++;
			GameMenu.Money -= Upgrades["Cost"][Level];
			SetObjectValues();
		}
		
	}
}
