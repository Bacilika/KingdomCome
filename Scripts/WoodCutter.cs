using Godot;
using System;
using System.Collections.Generic;

public partial class WoodCutter : Production{
	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private bool _timerTimedOut = false;
	private Timer _timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		_timer = GetNode<Timer>("WoodTimer");
		Price = 20000;

		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [0,0,0]}, {"WoodCost", [5, 10, 20]},
			{"StoneCost", [5, 10, 20]}, {"MoneyBackOnDelete", [4000, 2000, 2000] },{"WoodBackOnDelete", [3, 7, 15]},
			{"StoneBackOnDelete", [3, 7, 15]}, {"WoodMoveCost", [2, 5, 10]},
			{"StoneMoveCost", [2, 5, 10]}
		};
	}
	
	protected override void Tick()
	{
		if (Workers < Upgrades["MaxWorkers"][Level] && GameLogistics.WorkingCitizens < GameLogistics.Citizens)
		{
			if (_timer.IsStopped())
			{
				_timer.Start();
			}
			if (habitantGrowth.RandiRange(0, _growth) ==0)
			{
				Workers++;
				GameLogistics.WorkingCitizens++;
			}
		}
		UpdateInfo();
	}
}
