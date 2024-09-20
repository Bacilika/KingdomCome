using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
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
	protected Dictionary<String, List<int>> Upgrades; 
	protected AnimatedSprite2D AnimatedSprite;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		InfoBox.Visible = false;
		InfoBox.MoveToFront();
		Monitoring = true;
		_Ready_instance();
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
		AnimatedSprite = GetNode<AnimatedSprite2D>("HouseSprite");
	}

	public abstract void _Ready_instance();

	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			//Console.WriteLine("Mouse Entered");
			_isFocused = true;
		}			
	}
	
	public void OnMouseExited()
	{
		//Console.WriteLine("Mouse Exited");
		_isFocused = false;
	}
	
	public void OnAreaEntered(Area2D other)
	{
		//Console.WriteLine("Area Entered");
		if(IsPlaced)
		{
			GameMenu.ContainHouse = true;
		}			
	}
	
	public void OnAreaExited(Area2D other)
	{
		//Console.WriteLine("Area Exited");
		GameMenu.ContainHouse = false;
	}
	
	protected void FollowMouse()
	{
		Position = GetGlobalMousePosition();
	}
	
	
	
	public int GetPrice()
	{
		return Price;
	}
	
	

	public override void _Input(InputEvent @event)
	{
		if (_isFocused) //if mouse is on Building
		{
			if( @event.IsActionPressed(Inputs.LeftClick))
			{
				if (!GameMenu.IsPlaceMode)
				{
					InfoBox.Visible = !InfoBox.Visible;
				}
			}
		}
	}
	protected abstract void OnDelete();

	protected void OnUpgrade()
	{
		if (Level <_maxLevel && Upgrades["Cost"][Level] < GameMenu.Money)
		{
			Level++;
			AnimatedSprite.Frame = Level;
			Price = Upgrades["Cost"][Level];
			GameMenu.Money -= Upgrades["Cost"][Level];
			Shop.placeAudio.Play();
		}
	}
}
