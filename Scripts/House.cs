using Godot;
using System;

public partial class House : AbstractPlaceable
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(delta); 
		}
		
		
	}

}
