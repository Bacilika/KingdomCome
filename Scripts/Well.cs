using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Well : Production
{
	public override void _Ready_instance()
	{
		ProductionRate = 2;
		_timer = GetNode<Timer>("FoodTimer");
		_timer.Start();
		Price = 20000;
		Upgrades = new Dictionary<string, List<int>>
		{
			{Upgrade.Cost, [5000, 3000, 3000]}, {Upgrade.MaxWorkers, [1, 2, 3]},
			{Upgrade.Inhabitants, [0, 0, 0]}, {Upgrade.WoodCost, [1, 1, 1]},
			{Upgrade.StoneCost, [1, 1, 1]}, {Upgrade.MoneyBackOnDelete, [4000, 2000, 2000] },
			{Upgrade.WoodBackOnDelete, [3, 7, 15]}, {Upgrade.StoneBackOnDelete, [3, 7, 15]},
			{Upgrade.WoodMoveCost, [2, 5, 10]}, {Upgrade.StoneMoveCost, [2, 5, 10]}
		};
	}
	
	public override void ProduceItem()
	{
		GameLogistics.Resources["Water"]++;
	}

}
