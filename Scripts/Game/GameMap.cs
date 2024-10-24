using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Activities;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class GameMap : Node2D
{
	public static List<AbstractActivity> _placedActivities = [];
	public static List<LivingSpace> _placedHouses = [];
	public static List<Production> _placedProduction = [];
	
	public static List<Npc> Homeless = [];
	public static List<Npc> Unemployed = [];
	public List<Npc> Citizens = [];
	public static Dictionary<string, int> NpcStats  = new ();
	private TutorialWindow _tutorialWindow;

	//for job selection
	public static bool JobSelectMode;
	public static Npc NpcJobSelect;
	public static int Level = 1;
	private Timer _dayTimer;
	private Timer _foodTimer;
	public GameMenu _gameMenu;
	private AudioStreamPlayer2D _music;
	private PackedScene NPCScene;
	private PackedScene infoScene;
	public static bool TutorialMode = false;

	private TutorialWindow tutorial;

	public Timer GracePeriodTimer;
	private WorkBench _workBench;
	private double _timeSinceLastTick;
	public static string NpcStatsAsString;
	
	[Signal]
	public delegate void SendLogEventHandler(string log);
	[Signal]
	public delegate void DayOverEventHandler();

	public static bool GracePeriod = true;
	

	public override void _Ready()
	{
		tutorial = new TutorialWindow(this);
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
		GracePeriodTimer = new Timer();
		GracePeriodTimer.WaitTime = 60;
		GracePeriodTimer.OneShot = true;
		GracePeriodTimer.Timeout += () =>
		{
			GracePeriod = false;
			QueueFree();
		};
		_tutorialWindow = GetNode<TutorialWindow>("TutorialWindow");
		
		
		// Connect pause and play buttons
		GetNode<GameMenu>("GameMenu").Connect(GameMenu.SignalName.PauseButton, Callable.From(PauseGame));
		GetNode<GameMenu>("GameMenu").Connect(GameMenu.SignalName.PlayButton, Callable.From(PlayGame));
		
		// start workbench
		_workBench = GetNode<WorkBench>("WorkBench");
		_workBench.IsPlaced = true;
		_workBench.Visible = true;
		
		// Start NPC
		SpawnFirstNpc(GetNode<Npc>("Male"));
		SpawnFirstNpc(GetNode<Npc>("Female"));
		NpcStats = new Dictionary<string, int>()
		{
			{NpcStatuses.Unemployed, 0},
			{NpcStatuses.Citizens, 0},
			{NpcStatuses.Homeless, 0},
		};
	}

	public void SetNpcString()
	{
		NpcStatsAsString = "";
		foreach (var entry in NpcStats)
		{
			NpcStatsAsString += $"{entry.Key}: {entry.Value}\n";
		}
	}

	public override void _Process(double delta)
	{
		if(TutorialMode) _tutorialWindow.ShowTutorial();
		_timeSinceLastTick += delta;
		Unemployed = [];
		foreach (var npc in Citizens.Where(npc => npc.Work is null))
		{
			Unemployed.Add(npc);
			NpcStats[NpcStatuses.Unemployed] = Unemployed.Count;

		}
		Homeless = [];
		foreach (var npc in Citizens.Where(npc => npc.Home is null))
		{
			Homeless.Add(npc);
			NpcStats[NpcStatuses.Homeless] = Homeless.Count;
		}
		NpcStats[NpcStatuses.Citizens] = Citizens.Count;
		if (NpcStats[NpcStatuses.Unemployed] > 0) //there are unemployed
			GiveJobToNpcs();

		SetNpcString();

	}

	public void PauseGame()
	{
		_gameMenu.GetNode<ColorRect>("MenuCanvasLayer/PausedRect").Visible = true;
		GetTree().Paused = true;
	}
	
	public void PlayGame()
	{
		GetTree().Paused = false;
		_gameMenu.GetNode<ColorRect>("MenuCanvasLayer/PausedRect").Visible = false;
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
		else if (placeable is Decoration decoration)
		{
			decoration.DecorationsPoint += 1;
			if (decoration.DecorationsPoint == 5)
			{
				decoration.addDecorationPoint(Citizens);
			}
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
		SubscribeToSignals(npc);
		npc.Home = house;
		house.MoveIntoHouse(npc);
		npc.Position = house.Position;
		house.InfoBox.MoveToFront();
		Citizens.Add(npc);
		npc.OnJobChange += OnSelectJob;
		if (Citizens.Count % 10 == 0)
		{
			Level++;
			GameMenu.UpdateLevel(Level);
		}
	}

	private void SubscribeToSignals(Npc npc)
	{
		npc.SendLog += _gameMenu.GameLog.CreateLog;
		npc.OnFed += OnNpcFed;
		DayOver += npc.OnDayOver;
	}
	
	public void SpawnFirstNpc(Npc npc)
	{
		SubscribeToSignals(npc);
		Citizens.Add(npc);
		npc.OnJobChange += OnSelectJob;
		if (Citizens.Count % 10 + 2 == 0)
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
				if (job is WorkBench || !job.isDone)
					continue;
				if (job.HasMaxEmployees()) continue;
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
		TutorialWindow.CompleteTutorialStep("Select GiveJob");
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
	}
	public static bool HasUnemployedCitizens()
	{
		return NpcStats[NpcStatuses.Unemployed] > 0;
	}

	private void OnNpcFed(Npc npc, bool fed)
	{
		if (!fed)
		{
			EmitSignal(SignalName.SendLog,$" {npc.CitizenName} did not get fed today");
		}
	}
}
