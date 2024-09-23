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
	private bool _move = false;
	

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
		if (!IsPlaced) return;
		_time += delta;
		if (_time > 1)
		{
			_time -= 1;
			Tick();
		}
		
		InfoBox.MoveToFront();
		if (_move)
		{
			Position = GetGlobalMousePosition();
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
		
		else if (@event.IsActionPressed(Inputs.LeftClick) && GameMenu.ContainHouse == false && Upgrades["WoodMoveCost"][Level] <= GameMenu.Wood
				 && Upgrades["StoneMoveCost"][Level] <= GameMenu.Stone)
		{
			Console.WriteLine("left click");
			Vector2 position = GetGlobalMousePosition();
			GameMap.MoveHouse(this, GetGlobalMousePosition());
			_move = false;
			GameMenu.IsPlaceMode = false;
			GameMenu.Wood -= Upgrades["WoodMoveCost"][Level];
			GameMenu.Stone -= Upgrades["StoneMoveCost"][Level];
		}
	}
	protected abstract void OnDelete();

	protected void OnUpgrade()
	{
		if (Level <_maxLevel && Upgrades["WoodCost"][Level] < GameMenu.Wood
							 && Upgrades["StoneCost"][Level] < GameMenu.Stone)
		{
			EnoughSpace();
			Shop.placeAudio.Play();

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

	protected void OnMove()
	{
		InfoBox.Visible = false;
		_move = true;
		GameMenu.IsPlaceMode = true;
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
		await Task.Delay(100);
		GD.Print( GetOverlappingAreas().Count);
		if ( HasOverlappingAreas())
		{ 
			GD.Print("Collision when trying to upgrade");
			ActivateHitbox(Level); //return to old hitbox
		}
		else
		{
			GD.Print("No collision, upgrading house");
			Level++;
			GameMenu.Stone -= Upgrades["StoneCost"][Level];
			GameMenu.Wood -= Upgrades["WoodCost"][Level];
			SetObjectValues();
		}
	}
}
