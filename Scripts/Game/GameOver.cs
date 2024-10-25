using Godot;
using System;

public partial class GameOver : Control
{

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnPlayAgainButtonPressed()
	{
		//ToDo: Make replay work. 
		GetParent().GetParent().GetParent<GameMap>().GetTree().ReloadCurrentScene();
	}
	
	
}
