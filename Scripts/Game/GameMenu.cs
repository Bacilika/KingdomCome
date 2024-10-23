using System;
using System.Collections.Generic;
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
	public static Label NextLevel;
	public static ProgressBar LevelProgressbar;
	public CanvasLayer CanvasLayer;
	public ProductionInfo ProductionInfo;
	public GameLog GameLog;
	private static List<Npc> _citizen;
	public Shop Shop;
	public Button CancelButton;
	public bool CancelButtonFocused;
	public EventGenerator EventGenerator = new();
	private RandomNumberGenerator random = new();
	private Timer eventtimer = new Timer();

	
	[Signal]
	public delegate void PauseButtonEventHandler();
	
	[Signal]
	public delegate void PlayButtonEventHandler();
	

	public override void _Ready()
	{
		_citizen = GetParent<GameMap>().Citizens;
		ProductionInfo = GetNode<ProductionInfo>("MenuCanvasLayer/ProductionInfo");
		Day = GetNode<Label>("MenuCanvasLayer/GameStats/Day");
		Level = GetNode<Label>("MenuCanvasLayer/GameStats/Level/Level");
		NextLevel = GetNode<Label>("MenuCanvasLayer/GameStats/Level/NextLevel");
		LevelProgressbar = GetNode<ProgressBar>("MenuCanvasLayer/GameStats/Level/LevelProgress");
		GameMode = GetNode<Label>("MenuCanvasLayer/CurrentGameMode");
		CanvasLayer = GetNode<CanvasLayer>("MenuCanvasLayer");
		GameLog = GetNode<GameLog>("MenuCanvasLayer/GameLog");
		
		//Event 
		AddChild(eventtimer);
		int waitTime = random.RandiRange(5, 10);
		eventtimer.SetWaitTime(waitTime);
		eventtimer.Start();
		eventtimer.OneShot = false;
		eventtimer.Timeout += () =>
		{
			CanvasLayer.AddChild(EventGenerator.getEvent());
			int waitTime = random.RandiRange(5, 10);
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
		
		// game stats
		var statLabels = GetNode<HBoxContainer>("MenuCanvasLayer/GameStats");
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label>
		{
			{ RawResource.Money, statLabels.GetNode<TextureRect>(RawResource.Money).GetNode<Label>("Value") },
			{ RawResource.Food, statLabels.GetNode<TextureRect>(RawResource.Food).GetNode<Label>("Value") },
			{ RawResource.Citizens, statLabels.GetNode<TextureRect>(RawResource.Citizens).GetNode<Label>("Value") },
			{ RawResource.Stone, statLabels.GetNode<TextureRect>(RawResource.Stone).GetNode<Label>("Value") },
			{ RawResource.Happiness, statLabels.GetNode<TextureRect>(RawResource.Happiness).GetNode<Label>("Value") },
			{ RawResource.Wood, statLabels.GetNode<TextureRect>(RawResource.Wood).GetNode<Label>("Value") },
			{ RawResource.Unemployed, statLabels.GetNode<TextureRect>(RawResource.Unemployed).GetNode<Label>("Value") },
			{ RawResource.Iron, statLabels.GetNode<TextureRect>(RawResource.Iron).GetNode<Label>("Value") },
			{ RawResource.Water, statLabels.GetNode<TextureRect>(RawResource.Water).GetNode<Label>("Value") }
		};
		_gameStats = new Godot.Collections.Dictionary<string, TextureRect>
		{
			{ RawResource.Money, statLabels.GetNode<TextureRect>(RawResource.Money) },
			{ RawResource.Food, statLabels.GetNode<TextureRect>(RawResource.Food)},
			{ RawResource.Citizens, statLabels.GetNode<TextureRect>(RawResource.Citizens)},
			{ RawResource.Stone, statLabels.GetNode<TextureRect>(RawResource.Stone)},
			{ RawResource.Happiness, statLabels.GetNode<TextureRect>(RawResource.Happiness)},
			{ RawResource.Wood, statLabels.GetNode<TextureRect>(RawResource.Wood)},
			{ RawResource.Unemployed, statLabels.GetNode<TextureRect>(RawResource.Unemployed)},
			{ RawResource.Iron, statLabels.GetNode<TextureRect>(RawResource.Iron)},
			{ RawResource.Water, statLabels.GetNode<TextureRect>(RawResource.Water)}
		};
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();
	}

	public static void UpdateLevel(int updatedLevel)
	{
		LevelProgressbar.Value = 0;
		Level.Text =  $"Level: {updatedLevel.ToString()}";
		NextLevel.Text = (updatedLevel +1).ToString();

	}

	public void UpdateMenuInfo()
	{

		LevelProgressbar.Value = _citizen.Count % 10;
		Level.TooltipText = $"{10 - _citizen.Count % 10 } more citizen until next level";
		NextLevel.TooltipText = $"{10 - _citizen.Count % 10 } more citizen until next level";
		LevelProgressbar.TooltipText = $"{10 - _citizen.Count % 10 } more citizen until next level";

		Day.Text = "Day: " + GameLogistics.Day;

		foreach (var item in _gameStatLabels)
		{

			SetToolTip(item.Key);
			item.Value.Text = GameLogistics.Resources[item.Key].ToString();
		}
	}

	public void HideShop(bool hide)
	{
		CancelButton.Visible = hide;
		Shop.Visible = !hide;
	}

	private void SetToolTip(string key)
	{
		switch (key)
		{
			case RawResource.Food:
				_gameStats[key].SetTooltipText(GameLogistics.FoodResourceAsString);
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
