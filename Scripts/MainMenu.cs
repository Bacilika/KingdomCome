using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public Vector2 CurrentScale { get; set; }
	public override void _Ready()
	{
		//GetTree().Root.Mode = Window.ModeEnum.Maximized;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 size = GetTree().Root.Size;
		Vector2 minsize = GetTree().Root.MinSize;
		CurrentScale = size / minsize;
	}
	
	

	public void OnStartGamePressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/base.tscn");
		Console.WriteLine("start");
	}
	public void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
