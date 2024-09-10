using Godot;
using System;

public abstract partial class AbstractPlaceable : Node2D
{
	public bool containHouse;

	public void OnMouseEntered()
	{
		Console.WriteLine("OnMouseEntered");
		containHouse = true;
		GD.Print("OnMouseEntered");
	}

	public void OnMouseExited()
	{
		Console.WriteLine("OnMouseExited");
		GD.Print("OnMouseExited");
		containHouse = false;
	}
	
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
