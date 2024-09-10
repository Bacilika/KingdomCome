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
			GameMenu.containHouse = true; 
			GD.Print("OnMouseEntered");
		}			
	}

	public void OnMouseExited()
	{
		GameMenu.containHouse = false; 
		Console.WriteLine("OnMouseExited");
		GD.Print("OnMouseExited");
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected void FollowMouse(double delta)
	{
		Position = GetGlobalMousePosition();
	}
}
