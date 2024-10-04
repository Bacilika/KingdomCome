using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class FarmHouse : Production
{
	
	private int _food;
	
	public override void _Ready_instance()
	{
		BuildingName = "Barn";
		BuildingDescription = "A Barn that produces food";
		Producing = "Food";
		_timer = GetNode<Timer>("FoodTimer");
		Upgrades = new Dictionary<string, List<int>>
		{
			{Upgrade.Cost, [5000, 3000, 3000]}, {Upgrade.MaxWorkers, [5, 7, 10]},
			{Upgrade.Inhabitants, [5, 7, 10]}, {Upgrade.WoodCost, [2, 2, 2]},
			{Upgrade.StoneCost, [2, 2, 2]}, {Upgrade.MoneyBackOnDelete, [4000, 2000, 2000] },{Upgrade.WoodBackOnDelete, [3, 7, 15]},
			{Upgrade.StoneBackOnDelete, [3, 7, 15]}, {Upgrade.WoodMoveCost, [2, 5, 10]},
			{Upgrade.StoneMoveCost, [2, 5, 10]}
		};
	}


	public override void ProduceItem()
	{
		_food++;
		GameLogistics.Resources["Food"]++;
	}

}
