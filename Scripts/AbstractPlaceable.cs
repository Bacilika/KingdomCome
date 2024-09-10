using Godot;
using System;
using Scripts.Constants;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool IsPlaced;

	
	[Signal]
	public delegate void OnMouseInteractionEventHandler(bool interacted);
	
	public void OnMouseEntered()
	{
		if(IsPlaced)
		{
			GameMenu.ContainHouse = true;
			GameMenu.SelectedPlaceable = this;

		}			
	}

	public void OnMouseExited()
	{
		GameMenu.ContainHouse = false;
		GameMenu.SelectedPlaceable = null;

	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected void FollowMouse()
	{
		Position = GetGlobalMousePosition();
	}

	public int GetBuildingPrice()
	{
		return (int) GetType().GetMethod("GetPrice")!.Invoke(this, null)!;
	}

	public void ShowBuildingInfoScreen()
	{
		GetType().GetMethod("ShowInfo")?.Invoke(this, null);
	}

	
}
