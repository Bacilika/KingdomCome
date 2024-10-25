using System;
using Godot;

public partial class MainMenu : Control
{
	public Vector2 CurrentScale { get; set; }
	public static string KingdomName = "";

	public override void _Ready()
	{
		GetNode<LineEdit>("MenuOverlay/VBoxContainer2/LineEdit").FocusEntered += () =>
		{
			GetNode<Label>("MenuOverlay/VBoxContainer2/LineEdit/Label").Visible = false;
		};
		GetNode<Button>("MenuOverlay/VBoxContainer2/HBoxContainer/StartGame").Pressed += OnStartGame;
	}


	public void OnStartGamePressed()
	{
		GetNode<VBoxContainer>("MenuOverlay/VBoxContainer").Visible = false;
		GetNode<VBoxContainer>("MenuOverlay/VBoxContainer2").Visible = true;
		
	}
	public void OnStartGame()
	{
		if (GetNode<LineEdit>("MenuOverlay/VBoxContainer2/LineEdit").Text == "")
		{
			GetNode<Label>("MenuOverlay/VBoxContainer2/LineEdit/Label").Visible = true;
		}
		else
		{
			KingdomName = GetNode<LineEdit>("MenuOverlay/VBoxContainer2/LineEdit").Text;
			GetTree().ChangeSceneToFile("res://Scenes/Game/GameMap.tscn");
		}
	}

	public void OnQuitPressed()
	{
		GetTree().Quit();
	}
	public void OnCancelPressed()
	{
		GetNode<VBoxContainer>("MenuOverlay/VBoxContainer").Visible = true;
		GetNode<VBoxContainer>("MenuOverlay/VBoxContainer2").Visible = false;
	}

}
