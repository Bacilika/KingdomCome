using Godot;
using System;
using Godot.Collections;

public partial class TutorialWindow : Window
{
	public int tutorial = 1;
	public RichTextLabel Title;
	public RichTextLabel Description;
	public static Dictionary<string, bool> TutorialSteps = new();

	public override void _Ready()
	{
		
		Title = GetNode<RichTextLabel>("VBoxContainer/Title");
		Description = GetNode<RichTextLabel>("VBoxContainer/Description");
	}

	public static void CompleteTutorialStep(string key)
	{
		if (TutorialSteps.ContainsKey(key)) TutorialSteps[key] = true;
		
	}

	public void ShowTutorial()
	{
		switch (tutorial)
		{
			case 1: 
				ShowFirstTutorial();
				break;
			case 2: 
				ShowSecondTutorial();
				break;
			case 3: 
				ShowThirdTutorial();
				break;
			default:
				Console.WriteLine("No more Tutorials");
				break;
			
		}

		
	}

	public void ShowFirstTutorial()
	{
		Position =  (Vector2I) new Vector2(GetViewport().GetVisibleRect().Position.X + GetTree().Root.Size[0] -300,
			300);
		if (TutorialSteps.Count == 0)
		{
			TutorialSteps.Add("Select Npc",false );
			TutorialSteps.Add("Select GiveJob",false );
			TutorialSteps.Add("Employ Npc",false );
			TutorialSteps.Add("Build House",false );
		}

		if (!TutorialSteps["Select Npc"])
		{
			Description.Text = "You currently have 2 citizens. Click on them to see how they are doing";
		}
		else if (!TutorialSteps["Select GiveJob"])
		{
			Description.Text =
				"As you can see, your citizen is unemployed and homeless. To fix that, employ them to the workbench by clicking the \'Give Job\' button and select the workbench." ;
		}
		else if (!TutorialSteps["Employ Npc"])
		{
			Description.Text = "Click on the workbench while in \'Job Selection Mode\' to assign them to the workbench";
		}
		else if(!TutorialSteps["Build House"])
		{
			Description.Text =
				"When a citizen is employed at the workbench, they will help build the buildings you chose to place. " +
				"\n Press the \'House\' category and select a house to build";
		}
		else
		{
			Description.Text = "Well Done! Now your citizens have somewhere stay.";
		}

	}
	public void ShowSecondTutorial()
	{
		
	}
	public void ShowThirdTutorial()
	{
		
	}
}
