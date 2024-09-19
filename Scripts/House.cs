using Godot;
using System;
using System.Collections.Generic;

public partial class House : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private const int MaxHabitants = 5;
	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		Price = 5000;
		Upgrades = new Dictionary<String, List<int>>
		{
			{ "Cost", [5000, 3000, 3000] }, { "Inhabitants", [5, 7, 10] }, { "Workers", [5, 7, 10] },
			{ "WoodCost", [0, 0, 0] },
			{ "StoneCost", [0, 0, 0]}, {"MoneyBackOnDelete", [4000, 2000, 2000] }
		};
	}
	
	protected override void OnDelete()
	{
		Console.WriteLine("On delete");
		GameMenu.Citizens-= Citizens;
		QueueFree();
		GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
	}


	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
		}
		else
		{
			if (Citizens < MaxHabitants)
			{
				if (habitantGrowth.RandiRange(0, _growth) ==0)
				{
					Citizens++;
					GameMenu.Citizens++;
				}
			}
			UpdateInfo();
		}
		
	}


	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Citizens: " + Citizens;
	}
}
