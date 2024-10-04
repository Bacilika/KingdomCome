using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;

public partial class GameMenu : Control
{
	public static AudioStreamPlayer2D ButtonPress;
	private static Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	public static Label GameMode;
	public static Label Day;
	public static Label Level;
	public CanvasLayer CanvasLayer;
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		Day = GetNode<Label>("MenuCanvasLayer/Container/Day");
		Level = GetNode<Label>("MenuCanvasLayer/Container/Level");
		GameMode = GetNode<Label>("MenuCanvasLayer/CurrentGameMode");
		CanvasLayer = GetNode<CanvasLayer>("MenuCanvasLayer");
		var currentScale = (Vector2)GetTree().Root.Size / GetTree().Root.MinSize;
		var container = GetNode<Control>("MenuCanvasLayer/Container");
		container.Scale = currentScale;
		var statLabels = GetNode<GridContainer>("MenuCanvasLayer/Container/GameStats");
		ButtonPress = GetNode<AudioStreamPlayer2D>("ButtonPressedSound");
		ButtonPress?.Play();
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label> { 
			{"Money", statLabels.GetNode<TextureRect>("Money").GetNode<Label>("Value") },
			{"Food", statLabels.GetNode<TextureRect>("Food").GetNode<Label>("Value") },
			{"Citizens", statLabels.GetNode<TextureRect>("Citizens").GetNode<Label>("Value") },
			{"Stone",statLabels.GetNode<TextureRect>("Stone").GetNode<Label>("Value")}, 
			{"Happiness",statLabels.GetNode<TextureRect>("Happiness").GetNode<Label>("Value")},
			{"Wood",statLabels.GetNode<TextureRect>("Wood").GetNode<Label>("Value")},
			{"UnEmployed",statLabels.GetNode<TextureRect>("UnEmployed").GetNode<Label>("Value")},
			{"Iron",statLabels.GetNode<TextureRect>("Iron").GetNode<Label>("Value")},
			{"Water",statLabels.GetNode<TextureRect>("Water").GetNode<Label>("Value")}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();
	}

	public static void updateLevel(String updatedLevel)
	{
		Level.Text = "Level: " + updatedLevel;
	}

	public static void UpdateMenuInfo()
	{
		var red = new Color("#801917");
		
		Day.Text = "Day: " + GameLogistics.Day;
		
		foreach (var item in _gameStatLabels)
		{
			int value;

			switch (item.Key)
			{
				case "UnEmployed":
				{
					value = GameLogistics.Resources["UnEmployed"];
					break;
				}
				case "Citizens":
				{
					value = GameLogistics.Resources["Citizens"];
					break;
				}
				default:
				{
					value = GameLogistics.Resources[item.Key];
					if (value == 0)
					{
						item.Value.Set("theme_override_colors/font_color", red);
					}
					else
					{
						item.Value.Set("theme_override_colors/font_color", new Color(1, 1, 1));
					}

					break;
				}
			}
			
			item.Value.Text =": " + value;
		}

	}
}
