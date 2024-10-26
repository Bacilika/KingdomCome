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

public partial class TutorialWindow: Window
{
	public GameMap GameMap;
	public AnimatedSprite2D ArrowSprite;
	public RichTextLabel Title;
	public RichTextLabel Description;

	public static Dictionary<string, bool> TutorialSteps = new();

	public override void _Ready()
	{
		Visible = false;
		Title = GetNode<RichTextLabel>("VBoxContainer/Title");
		Description = GetNode<RichTextLabel>("VBoxContainer/Description");
		GameMap = GetParent<CanvasLayer>().GetParent<GameMenu>().GetParent<GameMap>();
		ArrowSprite = GameMap.GetNode<AnimatedSprite2D>("TutorialArrow");
		var screenPos = GetViewport().GetVisibleRect().Position.X;
		var screenSize = GetTree().Root.Size[0];
		Position =  (Vector2I) new Vector2(screenPos + screenSize -300, 300);
		Title.Text = "Royal Advisor";
		CloseRequested += () => Visible = false;
		GetNode<Button>("VBoxContainer/Button").Pressed += () => Visible = false;
	}
	

	public static void CompleteTutorialStep(string key)
	{
		if (CanBeCompleted(key))
		{
			if (TutorialSteps.ContainsKey(key)) TutorialSteps[key] = true;
		}
	}

	public void ShowTutorialWindow(string title, string description)
	{
		Visible = true;
		Title.Text = title;
		Description.Text = description;
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
		if (GetViewport() is null) return;
		
		var screenPos = GetViewport().GetVisibleRect().Position.X;
		var screenSize = GetTree().Root.Size[0];
		Position =  (Vector2I) new Vector2(screenPos + screenSize -300, 300);
		
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
			ArrowSprite.Position = GameMap.GetNode<Npc>("Male").Position + new Vector2(0,-50);
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
				ArrowSprite.Position = GameMap.GetNode<WorkBench>("WorkBench").Position+ new Vector2(0,-50);
				ArrowSprite.Play();
			}
		}
		else if (!TutorialSteps[TutorialStep.BuildHouse])
		{
		}
	}

}
