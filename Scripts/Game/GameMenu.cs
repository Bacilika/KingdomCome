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
			{"money", statLabels.GetNode<Label>("Money") },
			{"food", statLabels.GetNode<Label>("Food") },
			{"citizens", statLabels.GetNode<Label>("Citizens") },
			{"stone",statLabels.GetNode<Label>("Stone")}, 
			{"happiness",statLabels.GetNode<Label>("Happiness")},
			{"day",GetNode<Label>("MenuCanvasLayer/Container/Day")},
			{"wood",statLabels.GetNode<Label>("Wood")},
			{"Employed",statLabels.GetNode<Label>("Employed")},
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
		_gameStatLabels["money"].Text = "Money: " + GameLogistics.Resources["Money"];
		_gameStatLabels["citizens"].Text = "Citizens: " + GameLogistics.Resources["Citizens"];
		_gameStatLabels["happiness"].Text = "Happiness: " + GameLogistics.Resources["Happiness"];
		_gameStatLabels["food"].Text = "Food: " + GameLogistics.Resources["Food"];
		_gameStatLabels["stone"].Text = "Stone: " + GameLogistics.Resources["Stone"];
		_gameStatLabels["day"].Text = "Day " + GameLogistics.Day;
		_gameStatLabels["wood"].Text = "Wood: " + GameLogistics.Resources["Wood"];
		_gameStatLabels["Employed"].Text = "Employed: " + GameLogistics.Resources["WorkingCitizens"];
		_gameStatLabels["Iron"].Text = "  : " + GameLogistics.Resources["Iron"];
		_gameStatLabels["Water"].Text = "Water: " + GameLogistics.Resources["Water"];
	}


}
