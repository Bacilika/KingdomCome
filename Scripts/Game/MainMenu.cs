using Godot;

public partial class MainMenu : Control
{
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
		GetTree().ChangeSceneToFile("res://Scenes/Game/GameMap.tscn");
	}

	public void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
