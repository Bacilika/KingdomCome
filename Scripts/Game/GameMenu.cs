using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class GameMenu : Control
{
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);

	public static AudioStreamPlayer2D ButtonPress;
	private static Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	public static Label GameMode;
	public static Label Day;
	public static Label Level;
	public static Label NextLevel;
	public static ProgressBar LevelProgressbar;
	public CanvasLayer CanvasLayer;
	public ProductionInfo ProductionInfo;
	public GameLog GameLog;
	private static List<Npc> _citizen;


	public override void _Ready()
	{
		_citizen = GetParent<GameMap>().Citizens;
		ProductionInfo = GetNode<ProductionInfo>("MenuCanvasLayer/ProductionInfo");
		Day = GetNode<Label>("MenuCanvasLayer/Container/GameStats/Day");
		Level = GetNode<Label>("MenuCanvasLayer/Container/GameStats/Level/Level");
		NextLevel = GetNode<Label>("MenuCanvasLayer/Container/GameStats/Level/NextLevel");
		LevelProgressbar = GetNode<ProgressBar>("MenuCanvasLayer/Container/GameStats/Level/LevelProgress");
		GameMode = GetNode<Label>("MenuCanvasLayer/CurrentGameMode");
		CanvasLayer = GetNode<CanvasLayer>("MenuCanvasLayer");
		GameLog = GetNode<GameLog>("MenuCanvasLayer/GameLog");
		var currentScale = (Vector2)GetTree().Root.Size / GetTree().Root.MinSize;
		var container = GetNode<Control>("MenuCanvasLayer/Container");
		container.Scale = currentScale;
		var statLabels = GetNode<HBoxContainer>("MenuCanvasLayer/Container/GameStats");
		ButtonPress = GetNode<AudioStreamPlayer2D>("ButtonPressedSound");
		ButtonPress?.Play();
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label>
		{
			{ RawResource.Money, statLabels.GetNode<TextureRect>(RawResource.Money).GetNode<Label>("Value") },
			{ RawResource.Food, statLabels.GetNode<TextureRect>(RawResource.Food).GetNode<Label>("Value") },
			{ RawResource.Citizens, statLabels.GetNode<TextureRect>(RawResource.Citizens).GetNode<Label>("Value") },
			{ RawResource.Stone, statLabels.GetNode<TextureRect>(RawResource.Stone).GetNode<Label>("Value") },
			{ RawResource.Happiness, statLabels.GetNode<TextureRect>(RawResource.Happiness).GetNode<Label>("Value") },
			{ RawResource.Wood, statLabels.GetNode<TextureRect>(RawResource.Wood).GetNode<Label>("Value") },
			{
				RawResource.Unemployed,
				statLabels.GetNode<TextureRect>(RawResource.Unemployed).GetNode<Label>("Value")
			},
			{ RawResource.Iron, statLabels.GetNode<TextureRect>(RawResource.Iron).GetNode<Label>("Value") },
			{ RawResource.Water, statLabels.GetNode<TextureRect>(RawResource.Water).GetNode<Label>("Value") }
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
		Level.Text =  updatedLevel.ToString();
		NextLevel.Text = (updatedLevel +1).ToString();
	}

	public static void UpdateMenuInfo()
	{

		LevelProgressbar.Value = _citizen.Count % 10;

		Day.Text = "Day: " + GameLogistics.Day;

		foreach (var item in _gameStatLabels)
		{
			int value;
			if (item.Key == RawResource.Food)
			{
				item.Value.TooltipText = GameLogistics.FoodResourceAsString;
			}

			switch (item.Key)
			{
				case RawResource.Unemployed:
				{
					value = GameLogistics.Resources[RawResource.Unemployed];
					break;
				}
				case RawResource.Citizens:
				{
					value = GameLogistics.Resources[RawResource.Citizens];
					break;
				}
				default:
				{
					value = GameLogistics.Resources[item.Key];
					break;
				}
			}
			item.Value.Text = ": " + value;
		}
	}
}
