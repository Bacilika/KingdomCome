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
	public Timer _dayTimer;
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
	
	//Allowing children to be born
	public bool ChildIsBorn = false;
	private RandomNumberGenerator _rand = new();
	private PackedScene _npcScene1;


	
	[Signal]
	public delegate void SendLogEventHandler(string log);
	[Signal]
	public delegate void DayOverEventHandler();
	[Signal]
	public delegate void HomelessNpcEventHandler(Npc npc);

	public static bool GracePeriod = true;
	

	public override void _Ready()
	{
		TutorialMode = true;
		NpcStats = new Dictionary<string, int>()
		{
			{NpcStatuses.Unemployed, 0},
			{NpcStatuses.Citizens, 0},
			{NpcStatuses.Homeless, 0},
		};
		tutorial = new TutorialWindow(this);
		tutorial.ShowTutorial();
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
		
		
		// Connect pause and play buttons
		GetNode<GameMenu>("GameMenu").Connect(GameMenu.SignalName.PauseButton, Callable.From(PauseGame));
		GetNode<GameMenu>("GameMenu").Connect(GameMenu.SignalName.PlayButton, Callable.From(PlayGame));
		
		// start workbench
		_workBench = GetNode<WorkBench>("WorkBench");
		_workBench.IsPlaced = true;
		_workBench.Visible = true;
		_workBench.ZIndex = 0;
		
		// Start NPC
		SpawnFirstNpc(GetNode<Npc>("Male"));
		SpawnFirstNpc(GetNode<Npc>("Female"));

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
		
		_timeSinceLastTick += delta;
		if (_timeSinceLastTick >= 0.5)
		{
			_timeSinceLastTick -= 0.5;
			if(TutorialMode) tutorial.ShowTutorial();
		}
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
		//checkNpcIsNotNull();
	}

	public void checkNpcIsNotNull()
	{
		int deadc = 0;
		foreach (var npc in Citizens)
		{
			if (npc.dead)
			{
				deadc++;
			}
		}
		if (Citizens.Count == deadc)
		{
			_gameMenu.GameOver();
		}
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
		switch (placeable)
		{
			case LivingSpace livingSpace:
				_placedHouses.Add(livingSpace);
				break;
			case AbstractActivity activity:
				_placedActivities.Add(activity);
				break;
			case Decoration decoration:
			{
				decoration.DecorationsPoint += 1;
				if (decoration.DecorationsPoint == 5)
				{
					decoration.addDecorationPoint(Citizens);
				}

				break;
			}
		}

		if (placeable is Production production) _placedProduction.Add(production);
		placeable.IsPlaced = true;
		AddChild(placeable);
		placeable.HouseSprite.SetAnimation("Building"); 
		
		//progress bar
		if (placeable is not WorkBench)
		{
			placeable.AddChild(placeable.BuildingProgressBar);
			placeable.BuildingProgressBar.MinValue = 0;
			placeable.BuildingProgressBar.GlobalPosition = placeable.GlobalPosition + new Vector2(-50, -60);
			placeable.BuildingProgressBar.MaxValue = 25;
			placeable.BuildingProgressBar.Visible = true;
			placeable.BuildingProgressBar.ZIndex = 4;
			placeable.BuildingProgressBar.ShowPercentage = false;
			var theme = GD.Load<Theme>("res://Themes/Theme.tres");
			placeable.BuildingProgressBar.Theme = theme;
			placeable.BuildingProgressBar.Size = new Vector2(placeable.BuildingProgressBar.Size.X, 15); 
			placeable.BuildingProgressBar.SetCustomMinimumSize(new Vector2(100, 30));
		}

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
		npc.OnHomelessNpc += OnNpcHomeless;
		DayOver += npc.OnDayOver;
	}

	public void OnNpcHomeless(Npc npc)
	{
		foreach (var house in _placedHouses)
		{
			if (house.isDone && house.Inhabitants < house.Upgrades[Upgrade.MaxInhabitants][house.Level])
			{
				house.MoveIntoHouse(npc);
				house.Inhabitants++;
				Console.WriteLine(house.Inhabitants);
				Console.WriteLine(house.Upgrades[Upgrade.MaxInhabitants][Level]);
				return;
			}
		}
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
		int birth = _rand.RandiRange(0, 1);
		
		foreach (var house in _placedHouses)
		{
			if (house.isDone && house.Inhabitants < house.Upgrades[Upgrade.MaxInhabitants][house.Level] && ChildIsBorn)
			{
				_npcScene1 = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
				Npc npc = _npcScene1.Instantiate<Npc>();
				if (1 < house.Inhabitants)
				{
					AddChild(npc);
					SpawnFirstNpc(npc);
					return;
				}
				else
				{
					AddChild(npc);
					SpawnFirstNpc(npc);
					_npcScene1 = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
					Npc npc2 = _npcScene1.Instantiate<Npc>();
					AddChild(npc2);
					SpawnFirstNpc(npc2);
					EmitSignal(SignalName.SendLog,$" {npc.CitizenName} and {npc2.CitizenName} just moved in to your village!");
					return;
				}
				
			}
			
		}
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
