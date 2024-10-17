using System;
using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class GameMap : Node2D
{
	public static List<AbstractActivity> _placedActivities = [];
	public static List<LivingSpace> _placedHouses = [];
	public static List<Production> _placedProduction = [];
	public List<Npc> Citizens = [];
	public List<Npc> Homeless = [];

	//for job selection
	public static bool JobSelectMode;
	public static Npc NpcJobSelect;
	public static int Level = 1;
	private Timer _dayTimer;
	private Timer _foodTimer;
	private GameMenu _gameMenu;
	private AudioStreamPlayer2D _music;
	private PackedScene NPCScene;
	private PackedScene infoScene;

	private WorkBench _workBench;
	private double _timeSinceLastTick;


	[Signal]
	public delegate void SendLogEventHandler(string log);
	[Signal]
	public delegate void DayOverEventHandler();
	


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
		SendLog += _gameMenu.GameLog.CreateLog;
		NPCScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
		infoScene = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenInfo.tscn");

		GetNode<GameMenu>("GameMenu").Connect(GameMenu.SignalName.PauseButton, Callable.From(PauseGame));
		
		//start workbench
		_workBench = GetNode<WorkBench>("WorkBench");
		_workBench.IsPlaced = true;
		_workBench.Visible = true;
		
		//Start NPC
		PlaceNpc(GetNode<Npc>("Male"));
		PlaceNpc(GetNode<Npc>("Female"));
	}

	public override void _Process(double delta)
	{
		_timeSinceLastTick += delta;
		if (GameLogistics.Resources[RawResource.Unemployed] > 0) //there are unemployed
			GiveJobToNpcs();
	}

	private void PauseGame()
	{
		foreach (Npc npc in Citizens)
		{
			npc.ScheduleTimer.Paused = true;
			npc.AtWorkTimer.Paused = true;
			npc._move = false;
			_dayTimer.Paused = true;
		}
	}

	private void OnBackgroundMusicFinish()
	{
		_music.Seek(0);
		_music.Play();
	}

	private void OnEatFoodTimerTimedout()
	{
	}

	public void PlaceBuilding(Node2D nodeObject)
	{
		var placeable = (AbstractPlaceable)nodeObject;
		_workBench.BuildList.Add(placeable, []);
		if (placeable is LivingSpace livingSpace)
		{
			livingSpace.OnCreateNpc += PlaceNpc;
			_placedHouses.Add(livingSpace);
		}
		else if (placeable is AbstractActivity activity)
		{
			_placedActivities.Add(activity);
		}

		if (placeable is Production production) _placedProduction.Add(production);
		placeable.IsPlaced = true;
		//placeable.Position = GetGlobalMousePosition();
		AddChild(placeable);
		placeable.HouseSprite.SetAnimation("Building"); 
		EmitSignal(SignalName.SendLog,$"Successfully built {placeable.BuildingName}");
	}

	public void PlaceNpc(LivingSpace house)
	{
		var npc = NPCScene.Instantiate<Npc>();
		AddChild(npc);
		npc.SendLog += _gameMenu.GameLog.CreateLog;
		npc.OnFed += OnNpcFed;
		DayOver += npc.OnDayOver;
		npc.Home = house;
		house.MoveIntoHouse(npc);
		npc.CitizenName += $" {house.HouseholdName}";
		npc.EmitSignal(SignalName.SendLog, $"{npc.CitizenName} moved into house!");
		npc.Position = house.Position;
		house.InfoBox.MoveToFront();
		npc.ZIndex = 1;
		Citizens.Add(npc);


		GameLogistics.Resources[RawResource.Unemployed]++;

		npc.OnJobChange += OnSelectJob;
		if (Citizens.Count % 10 == 0)
		{
			Level++;
			GameMenu.UpdateLevel(Level);
		}
	}
	
	public void PlaceNpc(Npc npc)
	{
		var info = infoScene.Instantiate<CitizenInfo>();
		_gameMenu.CanvasLayer.AddChild(info);
		info.Visible = false;
		npc.SendLog += _gameMenu.GameLog.CreateLog;
		npc.OnFed += OnNpcFed;
		DayOver += npc.OnDayOver;
		npc.EmitSignal(SignalName.SendLog, $"{npc.CitizenName} moved into house!");
		npc.Info = info;
		npc.ZIndex = 1;
		Citizens.Add(npc);
		Homeless.Add(npc);
		_workBench.EmployWorker(npc);
		

		GameLogistics.Resources[RawResource.Unemployed]++;

		npc.OnJobChange += OnSelectJob;
		if (Citizens.Count % 10 == 0)
		{
			Level++;
			GameMenu.UpdateLevel(Level);
		}
	}

	private void GiveJobToNpcs()
	{
		foreach (var citizen in Citizens)
		{
			if (citizen.IsEmployed()) continue;
			Production closestJob = null;
			foreach (var job in _placedProduction)
			{
				if (job is WorkBench)
					continue;
				if (job.HasMaxEmployees() || !job.isDone) continue;
				//if jop is closer
				closestJob = job;
			}

			if (closestJob is not null)
				//citizen.GetJob(closestJob);
				closestJob.EmployWorker(citizen);
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
		var placeable = (AbstractPlaceable)nodeObject;
		placeable.IsPlaced = true;
		placeable.Position = position;
		//Fix
		Shop.placeAudio.Play();
	}

	private void OnDayTimerTimeout()
	{
		EmitSignal(SignalName.SendLog,$"Day {GameLogistics.Day} has passed");
		GameLogistics.Day += 1;
		EmitSignal(SignalName.DayOver);
		if (GameLogistics.Resources[RawResource.Food] > 0) GameLogistics.Resources[RawResource.Food] -= 1;
		
		
	}

	private void OnNpcFed(Npc npc, bool fed)
	{
		if (!fed)
		{
			EmitSignal(SignalName.SendLog,$" {npc.CitizenName} did not get fed today");
		}
	}
}
