using Godot;
using System;
using System.Collections.Generic;

public partial class FarmHouse : Production
{
	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	
	public override void _Ready_instance()
	{
		_timer = GetNode<Timer>("FoodTimer");
		Price = 20000;
		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [5, 7, 10]}, {"WoodCost", [20, 40, 100]},
			{"StoneCost", [40, 100, 100]}, {"MoneyBackOnDelete", [4000, 2000, 2000] },{"WoodBackOnDelete", [3, 7, 15]},
			{"StoneBackOnDelete", [3, 7, 15]}, {"WoodMoveCost", [2, 5, 10]},
			{"StoneMoveCost", [2, 5, 10]}
		};
	}

	protected override void Tick()
	{
		if (IsPlaced && _timer.IsStopped() && Workers > 0) 
		{
			_timer.Start();
		}
		if (Workers < Upgrades["MaxWorkers"][Level] && GameLogistics.WorkingCitizens < GameLogistics.Citizens)
		{
			if (habitantGrowth.RandiRange(0, _growth) ==0)
			{
				Workers++;
				GameLogistics.WorkingCitizens++;
			}
		}
		UpdateInfo();
	}

}
