using Godot;
using System;
using Godot.Collections;
using Scripts.Constants;

public static class TutorialStep
{
	public static string SelectNpc = "Select Npc";
	public static string SelectGiveJob = "Select GiveJob";
	public static string EmployNpc = "Employ Npc";
	public static string BuildHouse = "Build House";
}

public class TutorialWindow
{
	public GameMap GameMap;
	public AnimatedSprite2D ArrowSprite;
	public  Vector2I VOffset;
	public Vector2I HOffset;
	private float _timeSinceTick;

	public static Dictionary<string, bool> TutorialSteps = new();

	public TutorialWindow(GameMap gameMap)
	{
		VOffset = new Vector2I(0, -50);
		HOffset = new Vector2I(-50, 0);
		GameMap = gameMap;
		ArrowSprite = gameMap.GetNode<AnimatedSprite2D>("TutorialArrow");
	}

	public static void CompleteTutorialStep(string key)
	{
		if (CanBeCompleted(key))
		{
			if (TutorialSteps.ContainsKey(key)) TutorialSteps[key] = true;
		}
		
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
		_timeSinceTick += 0.5f;
		ArrowSprite.Visible = false;
		ArrowSprite.MoveToFront();
		ArrowSprite.RotationDegrees = 0;
		if (TutorialSteps.Count == 0)
		{
			TutorialSteps.Add(TutorialStep.SelectNpc, false);
			TutorialSteps.Add(TutorialStep.SelectGiveJob, false);
			TutorialSteps.Add(TutorialStep.EmployNpc, false);
			TutorialSteps.Add(TutorialStep.BuildHouse, false);
		}

		if (!TutorialSteps[TutorialStep.SelectNpc])
		{
			ArrowSprite.Visible = true;
			ArrowSprite.Position = GameMap.GetNode<Npc>("Male").Position + VOffset;
			ArrowSprite.Play();
		}
		else if (!TutorialSteps[TutorialStep.SelectGiveJob])
		{
			var citizeninfo = GameMap.GetNode<Npc>("Male").Info;
			var button = citizeninfo.GetNode<Button>("HBoxContainer/ChangeJob");
			if (citizeninfo.Visible)
			{
				button.SetPressedNoSignal(!button.IsPressed());
			}
			else
			{
				TutorialSteps[TutorialStep.SelectNpc] = false;
			}
		}
		else if (!TutorialSteps[TutorialStep.EmployNpc])
		{
			if (GameMenu.GameMode.Text == GameMode.JobChange)
			{
				ArrowSprite.Visible = true;
				ArrowSprite.Position = GameMap.GetNode<WorkBench>("WorkBench").Position+VOffset;
				ArrowSprite.Play();
			}
		}
		else if (!TutorialSteps[TutorialStep.BuildHouse])
		{
		}
	}

}
