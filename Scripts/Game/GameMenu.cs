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
			{ GameResource.Money, statLabels.GetNode<TextureRect>(GameResource.Money).GetNode<Label>("Value") },
			{ GameResource.Food, statLabels.GetNode<TextureRect>(GameResource.Food).GetNode<Label>("Value") },
			{ GameResource.Citizens, statLabels.GetNode<TextureRect>(GameResource.Citizens).GetNode<Label>("Value") },
			{ GameResource.Stone, statLabels.GetNode<TextureRect>(GameResource.Stone).GetNode<Label>("Value") },
			{ GameResource.Happiness, statLabels.GetNode<TextureRect>(GameResource.Happiness).GetNode<Label>("Value") },
			{ GameResource.Wood, statLabels.GetNode<TextureRect>(GameResource.Wood).GetNode<Label>("Value") },
			{
				GameResource.Unemployed,
				statLabels.GetNode<TextureRect>(GameResource.Unemployed).GetNode<Label>("Value")
			},
			{ GameResource.Iron, statLabels.GetNode<TextureRect>(GameResource.Iron).GetNode<Label>("Value") },
			{ GameResource.Water, statLabels.GetNode<TextureRect>(GameResource.Water).GetNode<Label>("Value") }
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
		var red = new Color("#801917");

		Day.Text = "Day: " + GameLogistics.Day;

		foreach (var item in _gameStatLabels)
		{
			int value;

			switch (item.Key)
			{
				case GameResource.Unemployed:
				{
					value = GameLogistics.Resources[GameResource.Unemployed];
					break;
				}
				case GameResource.Citizens:
				{
					value = GameLogistics.Resources[GameResource.Citizens];
					break;
				}
				default:
				{
					value = GameLogistics.Resources[item.Key];
					if (value == 0)
						item.Value.Set("theme_override_colors/font_color", red);
					else
						item.Value.Set("theme_override_colors/font_color", new Color(1, 1, 1));

					break;
				}
			}

			item.Value.Text = ": " + value;
		}
	}
}
