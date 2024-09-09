using Godot;
using System;

public partial class Base : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var gameMenu = GetNode<GameMenu>("GameMenu");
		gameMenu.HousePlaced += PlaceHouse;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlaceHouse(Node2D nodeObject)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		Console.WriteLine("Place House");
		placeable.IsPlaced = true;
		placeable.Position = GetGlobalMousePosition();
		AddChild(placeable);
		
	}
}
