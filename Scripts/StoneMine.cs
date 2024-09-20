using Godot;
using System;
using System.Collections.Generic;

public partial class StoneMine : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 100; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _stone;
	private RandomNumberGenerator stoneGrowth = new ();
	private int _stoneGrowth = 300; // 1/_growth% chance to increase habitants by 1 each tick. 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [5, 7, 10]}, {"WoodCost", [2, 2, 2]},
			{"StoneCost", [2, 2, 2]}, {"MoneyBackOnDelete", [4000, 2000, 2000] },{"WoodBackOnDelete", [3, 7, 15]},
			{"StoneBackOnDelete", [3, 7, 15]}
		};
	}
	
	
	protected override void OnDelete()
	{
		Console.WriteLine("On delete stonemine");
		GameMenu.WorkingCitizens -= Workers;
		QueueFree();
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameMenu.Wood += Upgrades["WoodBackOnDelete"][Level];
		GameMenu.Stone += Upgrades["StoneBackOnDelete"][Level];

	}
	
	

	protected override void Tick()
	{
		if (Workers < Upgrades["MaxWorkers"][Level] && GameMenu.WorkingCitizens < GameMenu.Citizens)
		{
			if (habitantGrowth.RandiRange(0, _growth) ==0)
			{
				Workers++;
				GameMenu.WorkingCitizens++; 
			}
		}
		if (stoneGrowth.RandiRange(0, _stoneGrowth) == 0 && Workers > 0)
		{
			_stone++;
			GameMenu.Stone++;
		}
			
		UpdateInfo();
	}

	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers;
	}

}
