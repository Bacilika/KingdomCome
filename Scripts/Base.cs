using Godot;
using System;

public partial class Base : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private int hungry;
	private Timer timer; 
	
	public override void _Ready()
	{
		Console.WriteLine("hello");
		
		timer = GetNode<Timer>("EatFoodTimer");
		var gameMenu = GetNode<GameMenu>("GameMenu");
		gameMenu.HousePlaced += PlaceHouse;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (GameMenu.Food > 0)
		{
			timer.Start();
			hungry = 0;

		}
	}

	public void OnEatFoodTimerTimedout()
	{
		hungry++; 
		Console.WriteLine("Your citizens are Hungry!");
		if (hungry > 5)
		{
			Console.WriteLine("Game Over :(");
		}
	}

	public void PlaceHouse(Node2D nodeObject)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		placeable.IsPlaced = true;
		placeable.Position = GetGlobalMousePosition();
		AddChild(placeable);
		
	}
}
