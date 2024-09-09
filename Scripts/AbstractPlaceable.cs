using Godot;
using System;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool IsPlaced;
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
