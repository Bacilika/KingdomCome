using Godot;
using System;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool IsPlaced;
	private bool _isFocused;
	protected PlaceableInfo InfoBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<PlaceableInfo>("PlaceableInfo");
		InfoBox.Visible = false;
		_Ready_instance();
		InfoBox.Connect(PlaceableInfo.SignalName.OnDelete, Callable.From(OnDelete));
		InfoBox.Connect(PlaceableInfo.SignalName.OnUpgrade, Callable.From(OnUpgrade));
	}

	public abstract void _Ready_instance();

	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			GameMenu.ContainHouse = true;
			_isFocused = true;

		}			
	}

	protected abstract void OnDelete();
	protected abstract void OnUpgrade();
	
	public void OnMouseExited()
	{
		GameMenu.ContainHouse = false;
		_isFocused = false;
	}
	
	protected void FollowMouse()
	{
		Position = GetGlobalMousePosition();
	}
	
	public int GetBuildingPrice()
	{
		return (int) GetType().GetMethod("GetPrice")!.Invoke(this, null)!;
	}
	
	public void UpgradeHouse()
	{
		GetNode<Sprite2D>("Sprite2D").SetTexture(GetNode<Texture2D>("res://Art/Buildings/House_Hay_Stone_2.png"));
	}

	private void ToggleBuildingInfo()
	{
		GetType().GetMethod("ShowInfo")?.Invoke(this, null);
	}

	public override void _Input(InputEvent @event)
	{
		if (_isFocused)
		{
			if( @event.IsActionPressed(Inputs.LeftClick))
			{
				if (!GameMenu.IsPlaceMode)
				{
					ToggleBuildingInfo();
				}
			}
		}
	}
}
