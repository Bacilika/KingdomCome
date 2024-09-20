using Godot;
using System;

public partial class Base : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private int hungry;
	private Timer _foodTimer; 
	private Timer _dayTimer;
	private AudioStreamPlayer2D _music;
	
	public override void _Ready()
	{
		Console.WriteLine("hello");
		
		_foodTimer = GetNode<Timer>("EatFoodTimer");
		_dayTimer = GetNode<Timer>("DayTimer");
		_dayTimer.Start();
		var gameMenu = GetNode<GameMenu>("GameMenu");
		gameMenu.HousePlaced += PlaceHouse;
		_music = GetNode<AudioStreamPlayer2D>("BackgroundMusic");
		_music.Play();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (GameMenu.Food > 0 && GameMenu.Citizens > 0)
		{
			_foodTimer.Start();
			hungry = 0;
		}
	}

	private void OnBackgroundMusicFinish()
	{
		_music.Seek(0);
		_music.Play();
		Console.WriteLine("Music");
	}

	private void OnEatFoodTimerTimedout()
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

	private void OnDayTimerTimeout()
	{
		GameMenu.Day += 1;
		if (GameMenu.Food > 0)
		{
			GameMenu.Food -= 1;
		}
		GameMenu.UpdateMenuInfo();
	}
}
