using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts;

public partial class GameMap : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private int hungry;
	private Timer _foodTimer; 
	private Timer _dayTimer;
	private AudioStreamPlayer2D _music;
	public List<House> _placedHouses = [];
	public List<Production> _placedProduction = [];
	
	public override void _Ready()
	{
		Console.WriteLine(GetWorld2D().GetNavigationMap());
		Console.WriteLine(GetNode<NavigationRegion2D>("NavigationRegion2D").GetNavigationMap());
		_foodTimer = GetNode<Timer>("EatFoodTimer");
		_dayTimer = GetNode<Timer>("DayTimer");
		_dayTimer.Start();
		var gameLogistics = GetNode<GameLogistics>("GameLogistics");
		gameLogistics.HousePlaced += PlaceHouse;
		_music = GetNode<AudioStreamPlayer2D>("BackgroundMusic");
		_music.Play();
		var nav = GetNode<NavigationRegion2D>("NavigationRegion2D");
		nav.BakeNavigationPolygon();
		Console.WriteLine(nav.IsBaking());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (GameLogistics.Food > 0 && GameLogistics.Citizens > 0)
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
		if (placeable is House house)
		{
			house.OnCreateNpc += PlaceNpc;
			_placedHouses.Add(house);
		}
		else 
		{
			_placedProduction.Add((Production)placeable);
		}
		placeable.IsPlaced = true;
		placeable.Position = GetGlobalMousePosition();
		AddChild(placeable);
	}

	public void PlaceNpc(House house)
	{
		var NPCScene = ResourceLoader.Load<PackedScene>("res://Scenes/NPC.tscn");
		var npc = NPCScene.Instantiate<Npc>();
		AddChild(npc);
		npc.Home = house;
		npc.Position = house.Position;
		npc.SetStartPos(npc.Position);
		foreach (var workplace in _placedProduction)
		{
			workplace.LookingForWorkers += npc.GetJob;
		}
	}
	
	
	public static void MoveHouse(Node2D nodeObject, Vector2 position)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		placeable.IsPlaced = true;
		placeable.Position = position;
		
	}

	private void OnDayTimerTimeout()
	{
		GameLogistics.Day += 1;
		if (GameLogistics.Food > 0)
		{
			GameLogistics.Food -= 1;
		}
		GameMenu.UpdateMenuInfo();
	}
}
