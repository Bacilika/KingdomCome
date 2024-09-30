using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;

public partial class GameMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	
	public static AudioStreamPlayer2D ButtonPress;

	
	private static Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	public static Label GameMode;
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		GameMode = GetNode<Label>("MenuCanvasLayer/CurrentGameMode");
		var currentScale = (Vector2)GetTree().Root.Size / GetTree().Root.MinSize;
		var container = GetNode<Control>("MenuCanvasLayer/Container");
		container.Scale = currentScale;
		var statLabels = GetNode<GridContainer>("MenuCanvasLayer/Container/GameStats");
		ButtonPress = GetNode<AudioStreamPlayer2D>("ButtonPressedSound");
		ButtonPress?.Play();
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label> { 
			{"Money", statLabels.GetNode<Label>("Money") },
			{"Food", statLabels.GetNode<Label>("Food") },
			{"Citizens", statLabels.GetNode<Label>("Citizens") },
			{"Stone",statLabels.GetNode<Label>("Stone")}, 
			{"Happiness",statLabels.GetNode<Label>("Happiness")},
			{"Day",GetNode<Label>("MenuCanvasLayer/Container/Day")},
			{"Wood",statLabels.GetNode<Label>("Wood")},
			{"WorkingCitizens",statLabels.GetNode<Label>("Employed")},
			{"Iron",statLabels.GetNode<Label>("Iron")},
			{"Water",statLabels.GetNode<Label>("Water")}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();
	}

	public static void UpdateMenuInfo()
	{
		var red = new Color("#801917");
		foreach (var item in _gameStatLabels)
		{
			int value;

			switch (item.Key)
			{
				case "Day":
				{
					value = GameLogistics.Day;
					break;
				}
				case "WorkingCitizens":
				{
					value = GameLogistics.Resources["WorkingCitizens"];
					if (value < GameLogistics.Resources["Citizens"])
					{
						item.Value.Set("theme_override_colors/font_color", red);
					}
					else
					{
						item.Value.Set("theme_override_colors/font_color", new Color(1, 1, 1));
					}
					
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
			item.Value.Text = item.Key + ": " + value;
		}

	}
}
