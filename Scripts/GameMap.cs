using Godot;
using System;
using System.Collections.Generic;

public partial class GameMap : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private int hungry;
	private Timer _foodTimer; 
	private Timer _dayTimer;
	private AudioStreamPlayer2D _music;
	private List<House> _placedHouses = [];
	private List<AbstractPlaceable> _placedProduction = [];
	
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
		if (placeable is House house)
		{
			_placedHouses.Add(house);
			PlaceNPC();
		}
		else
		{
			_placedProduction.Add(placeable);
		}
		placeable.IsPlaced = true;
		placeable.Position = GetGlobalMousePosition();
		AddChild(placeable);
	}

	public void PlaceNPC()
	{
		var NPCScene = ResourceLoader.Load<PackedScene>("res://Scenes/NPC.tscn");
		var npc = NPCScene.Instantiate<Npc>();
		AddChild(npc);
		npc.Position = new Vector2(0, 0);
		npc.setDestination(_placedProduction.Count > 0 ? _placedProduction[0].Position: new Vector2(2,2));

	}
	
	public static void MoveHouse(Node2D nodeObject, Vector2 position)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		placeable.IsPlaced = true;
		placeable.Position = position;
		
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
