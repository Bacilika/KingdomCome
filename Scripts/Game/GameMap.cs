using System;
using System.Collections.Generic;
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
	public List<Npc> Citizens = [];
	public List<Npc> Homeless = [];
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
	public static bool TutorialMode = true;

	public Timer GracePeriodTimer;
	private WorkBench _workBench;
	private double _timeSinceLastTick;
	
	[Signal]
	public delegate void SendLogEventHandler(string log);
	[Signal]
	public delegate void DayOverEventHandler();

	public static bool GracePeriod = true;
	

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
	}

	public override void _Process(double delta)
	{
		if(TutorialMode) _tutorialWindow.ShowTutorial();
		_timeSinceLastTick += delta;
		if (GameLogistics.Resources[RawResource.Unemployed] > 0) //there are unemployed
			GiveJobToNpcs();
	}

	private void PauseGame()
	{
		foreach (var npc in Citizens)
		{
			npc.ScheduleTimer.Paused = true;
			npc.AtWorkTimer.Paused = true;
			npc._move = false;
			_dayTimer.Paused = true;
		}
	}
	
	private void PlayGame()
	{
		foreach (Npc npc in Citizens)
		{
			npc.ScheduleTimer.Paused = false;
			npc.AtWorkTimer.Paused = false;
			npc._move = true;
			_dayTimer.Paused = false;
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


		GameLogistics.Resources[RawResource.Unemployed]++;

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
		Homeless.Add(npc);
		GameLogistics.Resources[RawResource.Unemployed]++;
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
