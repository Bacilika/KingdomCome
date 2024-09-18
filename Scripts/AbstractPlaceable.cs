using Godot;
using System;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool IsPlaced;
	private bool _isFocused;
	public Control InfoBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<Control>("PlaceableInfo");
		InfoBox.Visible = false;
		Console.WriteLine("Abstract placeable");
		_Ready_instance();
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



	public abstract void OnDelete();
	
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
