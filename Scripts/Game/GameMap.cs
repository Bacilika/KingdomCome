using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class GameMap : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private int hungry;
	private Timer _foodTimer; 
	private Timer _dayTimer;
	private AudioStreamPlayer2D _music;
	public List<LivingSpaces> _placedHouses = [];
	public List<Production> _placedProduction = [];
	public List<Npc> Citizens = [];
	public static List<AbstractActivity> _placedActivities = [];
	//for job selection
	public static bool JobSelectMode;
	public static Npc NpcJobSelect;
	public static int Level = 1;
	private GameMenu _gameMenu;

	private double _timeSinceLastTick;
	
	
	public override void _Ready()
	{
		_foodTimer = GetNode<Timer>("EatFoodTimer");
		_dayTimer = GetNode<Timer>("DayTimer");
		_dayTimer.Start();
		var gameLogistics = GetNode<GameLogistics>("GameLogistics");
		gameLogistics.HousePlaced += PlaceBuilding;
		_music = GetNode<AudioStreamPlayer2D>("BackgroundMusic");
		_music.Play();
		var nav = GetNode<NavigationRegion2D>("NavigationRegion2D");
		nav.BakeNavigationPolygon();
		_gameMenu = GetNode<GameMenu>("GameMenu");
	}
	
	public override void _Process(double delta)
	{

		_timeSinceLastTick += delta;
		if (GameLogistics.Resources[GameResource.Food] > 0 && GameLogistics.Resources[GameResource.Wood] > 0)
		{
			_foodTimer.Start();
			hungry = 0;
		}

		if (GameLogistics.Resources[GameResource.Unemployed] > 0) //there are unemployed
		{
			GiveJobToNpcs();
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

	public void PlaceBuilding(Node2D nodeObject)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		if (placeable is House house)
		{
			house.OnCreateNpc += PlaceNpc;
			_placedHouses.Add(house);
		}
		if (placeable is CityHouse cityHouse)
		{
			cityHouse.OnCreateNpc += PlaceNpc;
			_placedHouses.Add(cityHouse);
		}
		else if(placeable is AbstractActivity activity)
		{
			_placedActivities.Add(activity);
		}
		if(placeable is Production production)
		{
			_placedProduction.Add(production);
		}
		placeable.IsPlaced = true;
		placeable.Position = GetGlobalMousePosition();
		
		AddChild(placeable);
	}

	public void PlaceNpc(LivingSpaces house)
	{
		var NPCScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
		var infoScene = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenInfo.tscn");
		var npc = NPCScene.Instantiate<Npc>();
		var info = infoScene.Instantiate<CitizenInfo>();
		_gameMenu.CanvasLayer.AddChild(info);
		info.Visible = false;
		AddChild(npc);
		
		npc.Home = house;
		house.MoveIntoHouse(npc);
		npc.Position = house.Position;
		npc.SetStartPos(npc.Position);
		npc.Info = info;
		house.InfoBox.MoveToFront();
		npc.ZIndex = 1;
		Citizens.Add(npc);
		
		GameLogistics.Resources[GameResource.Unemployed]++;
		
		npc.OnJobChange += OnSelectJob;
		if (Citizens.Count % 10 == 0)
		{
			Level++;
			GameMenu.updateLevel(Level.ToString());
		}
	}

	private void GiveJobToNpcs()
	{
		foreach (var citizen in Citizens)
		{
			if(citizen.IsEmployed()) continue;
			Production closestJob = null;
			foreach (var job in _placedProduction)
			{
				if(job.HasMaxEmployees())continue;
				//if jop is closer
				closestJob = job;
			}

			if (closestJob is not null)
			{
				//citizen.GetJob(closestJob);
				closestJob.EmployWorker(citizen);
			}
		}
	}
	public void OnSelectJob(Npc npc)
	{
		JobSelectMode = true;
		NpcJobSelect = npc;
		GameMenu.GameMode.Text = GameMode.JobChange;
	}
	
	
	public static void MoveHouse(Node2D nodeObject, Vector2 position)
	{
		AbstractPlaceable placeable = (AbstractPlaceable) nodeObject;
		placeable.IsPlaced = true;
		placeable.Position = position;
		//Fix
		Shop.placeAudio.Play();
	}

	private void OnDayTimerTimeout()
	{
		GameLogistics.Day += 1;
		if (GameLogistics.Resources[GameResource.Food] > 0)
		{
			GameLogistics.Resources[GameResource.Food] -= 1;
		}
		GameMenu.UpdateMenuInfo();
	}
}
