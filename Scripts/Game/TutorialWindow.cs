using Godot;
using System;
using Godot.Collections;
using Scripts.Constants;

public class TutorialWindow
{
	public GameMap GameMap;
	public AnimatedSprite2D ArrowSprite;

	public static Dictionary<string, bool> TutorialSteps = new();

	public TutorialWindow(GameMap gameMap)
	{
		GameMap = gameMap;
		ArrowSprite = gameMap.GetNode<AnimatedSprite2D>("TutorialArrow");
	}

	public static void CompleteTutorialStep(string key)
	{
		if (TutorialSteps.ContainsKey(key)) TutorialSteps[key] = true;
	}

	public static bool CanBeCompleted(string key)
	{
		foreach (var entry in TutorialSteps)
		{
			if (entry.Key == key)
			{
				return true;
			}

			if (entry.Value == false)
			{
				return false;
			}
		}
		return false;
	}

	public void ShowTutorial()
	{
		if (TutorialSteps.Count == 0)
		{
			TutorialSteps.Add("Select Npc", false);
			TutorialSteps.Add("Select GiveJob", false);
			TutorialSteps.Add("Employ Npc", false);
			TutorialSteps.Add("Build House", false);
		}

		if (!TutorialSteps["Select Npc"])
		{
			ArrowSprite.Position = GameMap.GetNode<Npc>("Male").Position;
			ArrowSprite.Play();
		}
		else if (!TutorialSteps["Select GiveJob"])
		{
			var citizeninfo = GameMap.GetNode<Npc>("Male").Info;
			if (citizeninfo.Visible)
			{
				ArrowSprite.Position = citizeninfo.GetNode<Button>("HBoxContainer/ChangeJob").Position;
				ArrowSprite.Play();
			}
		}
		else if (!TutorialSteps["Employ Npc"])
		{
			if (GameMenu.GameMode.Text == GameMode.JobChange)
			{
				ArrowSprite.Position = GameMap.GetNode<WorkBench>("Workbench").Position;
				ArrowSprite.Play();
			}
		}
		else if (!TutorialSteps["Build House"])
		{
		}
	}
}
