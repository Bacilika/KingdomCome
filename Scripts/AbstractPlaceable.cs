using Godot;
using System;
using System.Threading;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Area2D
{
	public bool IsPlaced;
	private bool _isFocused;
	protected PlaceableInfo InfoBox;
	protected int Price;
	protected int Level;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		InfoBox.Visible = false;

		Monitoring = true;
		_Ready_instance();
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
	}

	public abstract void _Ready_instance();

	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			Console.WriteLine("Mouse Entered");
			_isFocused = true;
		}			
	}
	
	public void OnMouseExited()
	{
		Console.WriteLine("Mouse Exited");
		_isFocused = false;
	}
	
	public void OnAreaEntered(Area2D other)
	{
		Console.WriteLine("Area Entered");
		if(IsPlaced)
		{
			GameMenu.ContainHouse = true;
		}			
	}
	
	public void OnAreaExited(Area2D other)
	{
		Console.WriteLine("Area Exited");
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
	
	public void UpgradeHouse()
	{
		GetNode<Sprite2D>("Sprite2D").SetTexture(GetNode<Texture2D>("res://Art/Buildings/House_Hay_Stone_2.png"));
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
	protected abstract void OnUpgrade();
}
