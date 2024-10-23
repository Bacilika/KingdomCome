using Godot;

public partial class MainMenu : Control
{
	public Vector2 CurrentScale { get; set; }
	
	public void OnStartGamePressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Game/GameMap.tscn");
	}

	public void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
