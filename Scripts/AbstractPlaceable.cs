using Godot;
using System;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool IsPlaced;
	private bool _isFocused;
	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			GameMenu.ContainHouse = true;
			GameMenu.SelectedPlaceable = this;
			_isFocused = true;

		}			
	}
	
	public void OnMouseExited()
	{
		GameMenu.ContainHouse = false;
		GameMenu.SelectedPlaceable = null;
		_isFocused = false;

	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var infoBox = GetNode<Control>("PlaceableInfo");
		infoBox.Visible = false;

		
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
