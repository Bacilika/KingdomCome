using Godot;
using System;
using System.Collections.Generic;

public partial class HunterLodge : AbstractPlaceable
{

	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 2;// 1/_growth% chance to increase habitants by 1 each tick. 
	private int _food;
	private Timer _timer; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		_timer = GetNode<Timer>("FoodTimer");
		_timer.Start();
		Price = 20000;
		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [0, 0, 0]}, {"WoodCost", [1, 1, 1]},
			{"StoneCost", [1, 1, 1]}, {"MoneyBackOnDelete", [4000, 2000, 2000] }, {"WoodBackOnDelete", [3, 7, 15]},
			{"StoneBackOnDelete", [3, 7, 15]}, {"WoodMoveCost", [2, 5, 10]},
			{"StoneMoveCost", [2, 5, 10]}
		};
	}
	

	protected override void Tick()
	{
		if (Workers < Upgrades["MaxWorkers"][Level] && GameMenu.WorkingCitizens < GameMenu.Citizens)
		{
			if (_timer.IsStopped())
			{
				_timer.Start();
			}

			if (habitantGrowth.RandiRange(0, _growth) ==0)
			{
				Workers++;
				GameMenu.WorkingCitizens++;
			}
		}
		UpdateInfo();
	}

	protected override void OnDelete()
	{
		GameMenu.WorkingCitizens -= Workers;
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameMenu.Wood += Upgrades["WoodBackOnDelete"][Level];
		GameMenu.Stone += Upgrades["StoneBackOnDelete"][Level];
		QueueFree();
	}
	
	public void OnFoodTimerTimeout()
	{
		_food++;
		GameMenu.Food++;
		float time = 15 - Workers;
		_timer.Start(time);
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers;
	}
}
