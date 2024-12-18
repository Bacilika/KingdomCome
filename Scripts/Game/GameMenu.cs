using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Godot;
using Scripts.Constants;

public partial class GameMenu : Control
{
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);

	public static AudioStreamPlayer2D ButtonPress;
	private static Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	private static Godot.Collections.Dictionary<string, TextureRect> _gameStats;
	public static Label GameMode;
	public static Label Day;
	public static Label Level;
	public static ProgressBar LevelProgressbar;
	public static ProgressBar DayProgressbar;
	public CanvasLayer CanvasLayer;
	public ProductionInfo ProductionInfo;
	public GameLog GameLog;
	private static List<Npc> _citizen;
	public Shop Shop;
	public Button CancelButton;
	public bool CancelButtonFocused;
	public EventGenerator EventGenerator = new();
	private RandomNumberGenerator random = new();
	public Timer eventtimer = new Timer();
	public TutorialWindow TutorialWindow; 
	private double _tickCounter;
	
	//Game over
	private PackedScene _gameOverScene;


	
	[Signal]
	public delegate void PauseButtonEventHandler();
	
	[Signal]
	public delegate void PlayButtonEventHandler();
	

	public override void _Ready()
	{
		_citizen = GetParent<GameMap>().Citizens;
		ProductionInfo = GetNode<ProductionInfo>("MenuCanvasLayer/ProductionInfo");
		Day = GetNode<Label>("MenuCanvasLayer/GameStats/Day/Day");
		DayProgressbar = GetNode<ProgressBar>("MenuCanvasLayer/GameStats/Day/DayProgress");
		Level = GetNode<Label>("MenuCanvasLayer/GameStats/Level/Level");
		LevelProgressbar = GetNode<ProgressBar>("MenuCanvasLayer/GameStats/Level/LevelProgress");
		GameMode = GetNode<Label>("MenuCanvasLayer/CurrentGameMode");
		CanvasLayer = GetNode<CanvasLayer>("MenuCanvasLayer");
		GameLog = GetNode<GameLog>("MenuCanvasLayer/GameLog");
		var gameMap = GetParent<GameMap>();
		var timer = gameMap.GetNode<Timer>("DayTimer");
		DayProgressbar.MaxValue = timer.WaitTime;
		
		
		//Event timer
		AddChild(eventtimer);
		int waitTime = random.RandiRange(65, 125);
		eventtimer.SetWaitTime(waitTime);
		eventtimer.OneShot = false;
		eventtimer.Timeout += () =>
		{
			CanvasLayer.AddChild(EventGenerator.getEvent());
			int waitTime = random.RandiRange(60, 300);
			eventtimer.SetWaitTime(waitTime);
			eventtimer.Start();
		};
		
		// sounds
		ButtonPress = GetNode<AudioStreamPlayer2D>("ButtonPressedSound");
		ButtonPress?.Play();
		Shop = GetNode<Shop>("MenuCanvasLayer/Shop");
		
		// cancelbutton for shop
		CancelButton = GetNode<Button>("MenuCanvasLayer/CancelButton");
		CancelButton.Pressed += () =>
		{
			GetParent<GameMap>().GetNode<GameLogistics>("GameLogistics").ResetModes();
		};
		CancelButton.MouseEntered += () => { CancelButtonFocused = true; };
		CancelButton.MouseExited += () => { CancelButtonFocused = false; };

		// set up tutorial selection
		
		
		var introEvent = EventGenerator.CreateEvent(1);
		GetNode<CanvasLayer>("MenuCanvasLayer").AddChild(introEvent);
		GetParent().Ready += () =>
		{
			gameMap.PauseGame();
			introEvent.Visible = true;
		};
		
		introEvent.Title.Text = "Your New Kingdom";
		introEvent.Description.Text = "You and your partner once lived under the rule of a crumbling kingdom," +
									  " where neglect and corruption stifled hope and progress. " +
									  "Tired of watching the land and its people suffer, you dared to dream " +
									  "of something better: a kingdom built on fairness, strength, and vision. " +
									  "Together, you set out to create a place where every choice is yours, " +
									  "where people thrive, and dreams flourish. Though challenges will come, " +
									  "this time, the future is in your hands. Brick by brick, " +
									  "you will build a realm that stands as a beacon of hope—a kingdom truly " +
									  "worth fighting for.\nYour journey begins now.";
		
		introEvent.buttons[0].Text = "Continue";
		introEvent.buttons[0].Pressed += () =>
		{
			introEvent.Title.Text = "Your Royal Advisor";
			introEvent.Description.Text = "By your side stands your loyal advisor—a wise and steady guide, shaped by years" +
										  " of experience and dedication. With a keen eye for strategy and a heart aligned" +
										  " with your vision, he offers insights, counsel, and encouragement, helping you transform" +
										  " ideas into action as you build your new kingdom.";
		};
		introEvent.DoneButton.Visible = false;
		introEvent.ButtonContainer.Visible = true;
		introEvent.DoneButton.Pressed += () =>
		{
			introEvent.Gamemap.PlayGame();
			introEvent.GetParent().RemoveChild(introEvent);
			introEvent.QueueFree();
			
		};
		
		TutorialWindow = GetNode<TutorialWindow>("MenuCanvasLayer/TutorialWindow");

		
		// game stats
		var statLabels = GetNode<HBoxContainer>("MenuCanvasLayer/GameStats");
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label>
		{
			{ RawResource.Food, statLabels.GetNode<TextureRect>(RawResource.Food).GetNode<Label>("Value") },
			{ NpcStatuses.Citizens, statLabels.GetNode<TextureRect>(NpcStatuses.Citizens).GetNode<Label>("Value") },
			{ "Happiness", statLabels.GetNode<TextureRect>("Happiness").GetNode<Label>("Value") },
			{ "RawResources", statLabels.GetNode<TextureRect>("RawResources").GetNode<Label>("Value") },
			{ "ProcessedResources", statLabels.GetNode<TextureRect>("ProcessedResources").GetNode<Label>("Value") },
		};
		_gameStats = new Godot.Collections.Dictionary<string, TextureRect>
		{
			{ RawResource.Food, statLabels.GetNode<TextureRect>(RawResource.Food)},
			{ NpcStatuses.Citizens, statLabels.GetNode<TextureRect>(NpcStatuses.Citizens)},
			{ "Happiness", statLabels.GetNode<TextureRect>("Happiness")},
			{ "RawResources", statLabels.GetNode<TextureRect>("RawResources")},
			{ "ProcessedResources", statLabels.GetNode<TextureRect>("ProcessedResources")},
			
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_tickCounter += delta;
		if (_tickCounter >= 0.5)
		{
			_tickCounter -= 0.5;
			if (GameMap.TutorialMode) TutorialWindow.ShowTutorial();
		}
		UpdateMenuInfo();
	}

	public static void UpdateLevel(int updatedLevel)
	{
		LevelProgressbar.Value = 0;
		Level.Text =  $"Level: {updatedLevel.ToString()}";
		
	}

	public void GameOver()
	{
		_gameOverScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/GameOver.tscn");
		var gameOver = _gameOverScene.Instantiate<GameOver>();
		CanvasLayer.AddChild(gameOver);
		gameOver.Visible = true;
	}

	public void UpdateMenuInfo()
	{
		LevelProgressbar.Value = _citizen.Count % 5;
		Level.TooltipText = $"{5 - _citizen.Count % 5 } more citizen until next level";
		LevelProgressbar.TooltipText = $"{5 - _citizen.Count % 5 } more citizen until next level";

		Day.Text = "Day: " + GameLogistics.Day;
		DayProgressbar.Value = GetParent<GameMap>()._dayTimer.WaitTime - GetParent<GameMap>()._dayTimer.TimeLeft;

		foreach (var item in _gameStatLabels)
		{
			SetToolTip(item.Key);
			if (GameMap.NpcStats.TryGetValue(item.Key, out var stat))
			{
				item.Value.Text = stat.ToString();
				continue;
			}

			item.Value.Text = item.Key switch
			{
				"Happiness" => GameLogistics.ConvertHappiness(GameMap.KingdomHappiness),
				"RawResources" => (GameLogistics.Resources.Sum(resource => resource.Value) -
								   GameLogistics.Resources[RawResource.Food]).ToString(),
				"ProcessedResources" => GameLogistics.ProcessedResources.Sum(resource => resource.Value).ToString(),
				_ => GameLogistics.Resources[item.Key].ToString()
			};
		}
	}

	public void HideShop(bool hide)
	{
		CancelButton.Visible = hide;
		Shop.Visible = !hide;
	}

	private static void SetToolTip(string key)
	{
		switch (key)
		{
			case RawResource.Food:
				_gameStats[key].SetTooltipText("Foods\n" + GameLogistics.FoodResourceAsString);
				break;
			case NpcStatuses.Citizens:
				_gameStats[key].SetTooltipText("Citizen Info\n" + GameMap.NpcStatsAsString);
				break;
			case "RawResources":
				_gameStats[key].SetTooltipText( "Raw Resources\n" + GameLogistics.RawResourceAsString());
				break;
			case "ProcessedResources":
				_gameStats[key].SetTooltipText( "Processed Resources\n" + GameLogistics.ProcessedResourceAsString());
				break;
		}
	}

	private void OnPauseButtonPressed()
	{
		EmitSignal(SignalName.PauseButton);
	}
	
	private void OnPlayButtonPressed()
	{
		EmitSignal(SignalName.PlayButton);
	}
	
	
}
