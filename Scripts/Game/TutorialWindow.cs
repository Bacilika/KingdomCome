using Godot;
using System;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;
using Timer = Godot.Timer;

public static class TutorialStep
{
	public static string SelectNpc = "Select Npc";
	public static string SelectGiveJob = "Select GiveJob";
	public static string EmployNpc = "Employ Npc";
	public static string BuildHouse = "Build House";
	public static string BuildProduction = "Build Production Npc";
	public static string MoveCamera = "Move Camera";
}

public partial class TutorialWindow: Panel
{
	public GameMap GameMap;
	public AnimatedSprite2D ArrowSprite;
	public AnimatedSprite2D wasd;
	public static RichTextLabel Title;
	public static RichTextLabel Description;
	public GameMenu GameMenu;
	private static bool _hidden;

	public static Dictionary<string, bool> TutorialSteps = new();

	public override void _Ready()
	{
		Visible = false;
		Title = GetNode<RichTextLabel>("VBoxContainer/Title");
		Description = GetNode<RichTextLabel>("VBoxContainer/Description");
		GameMap = GetParent<CanvasLayer>().GetParent<GameMenu>().GetParent<GameMap>();
		GameMenu = GetParent<CanvasLayer>().GetParent<GameMenu>();
		ArrowSprite = GameMap.GetNode<AnimatedSprite2D>("TutorialArrow");
		wasd = GameMenu.GetNode<AnimatedSprite2D>("MenuCanvasLayer/Wasd");
		GetNode<Button>("VBoxContainer/Button").Pressed += () =>
		{
			_hidden = true;
			Visible = false;
		};
		wasd.Visible = false;
	}
	
	

	public static void CompleteTutorialStep(string key)
	{

		if (TutorialSteps.ContainsKey(key)) TutorialSteps[key] = true;
		_hidden = false;
	}

	public void ShowTutorialWindow(string title, string description)
	{
		if(!_hidden) Visible = true;
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

	public void TutorialDone()
	{
		GameMap.TutorialMode = false;
		GameMenu.eventtimer.Start();
	}

	public void ShowTutorial()
	{
		Visible = false;
		wasd.Visible = false;
		ArrowSprite.Visible = false;
		ArrowSprite.MoveToFront();
		ArrowSprite.RotationDegrees = 0;
		if (TutorialSteps.Count == 0)
		{
			TutorialSteps.Add(TutorialStep.MoveCamera, false);
			TutorialSteps.Add(TutorialStep.SelectNpc, false);
			TutorialSteps.Add(TutorialStep.SelectGiveJob, false);
			TutorialSteps.Add(TutorialStep.EmployNpc, false);
			TutorialSteps.Add(TutorialStep.BuildHouse, false);
			TutorialSteps.Add(TutorialStep.BuildProduction, false);
		}
		if (!TutorialSteps[TutorialStep.MoveCamera])
		{
			wasd.Visible = true;
			wasd.Play();
			
		}
		
		else if (!TutorialSteps[TutorialStep.SelectNpc])
		{
			ShowTutorialWindow("Royal Advisor",
				"\"Citizens that are assigned to the workbench can help you build buildings such as Housing and Production.\"");
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
			if (GameMenu.GameMode.Text != GameMode.JobChange) return;
			ArrowSprite.Visible = true;
			ArrowSprite.Position = GameMap.GetNode<WorkBench>("WorkBench").Position+ new Vector2(0,-50);
			ArrowSprite.Play();
		}
		else if (!TutorialSteps[TutorialStep.BuildHouse])
		{

			ShowTutorialWindow("Royal Advisor","\"Your Citizens need a House to sleep in when they are not at work.\"");
			ArrowSprite = GameMenu.GetNode<AnimatedSprite2D>("MenuCanvasLayer/TutorialArrow");
			if (!GameMenu.Shop.GetNode<ScrollContainer>("BuildTabButtons/Houses").Visible) return;
			if(!GameMenu.Shop.Visible) return;
			ArrowSprite.Visible = true;
				
			var position =GameMenu.Shop.GlobalPosition + new Vector2(40,-40);
			ArrowSprite.Position= position;
			ArrowSprite.Play();
		}
		else if (!TutorialSteps[TutorialStep.BuildProduction])
		{
			ShowTutorialWindow("Royal Advisor", "\"Your Citizens need food everyday to live. A good way to get food is through a Hunter's Lodge.\"");
			if (!GameMenu.Shop.GetNode<ScrollContainer>("BuildTabButtons/Productions").Visible) return;
			if(!GameMenu.Shop.Visible) return;
			
			ArrowSprite.Visible = true;
			var position = GameMenu.Shop.GlobalPosition + new Vector2(25,-40);
			ArrowSprite.Position= position;
			ArrowSprite.Play();
		}

		else
		{
			ShowTutorialWindow("Royal Advisor","\"Remember to keep track of your citizens happiness, or they might move out.\"");
			TutorialDone();
		}
	}
}
