using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class Tavern : AbstractActivity
{
	
	
	public override void _Ready_instance()
	{
		BuildingName = "Tavern";
		BuildingDescription = "A place for citizens to gather and drink beer. Requires water and foot";
		Price = 5000;
		Upgrades = new Dictionary<string, List<int>>
		{
			{Upgrade.Cost, [5000, 3000, 3000]}, {Upgrade.MaxWorkers, [3, 5, 7]},
			{Upgrade.Inhabitants, [5, 7, 10]}, {Upgrade.WoodCost, [2, 2, 2]},
			{Upgrade.StoneCost, [2, 2, 2]}, {Upgrade.MoneyBackOnDelete, [4000, 2000, 2000] },{Upgrade.WoodBackOnDelete, [3, 7, 15]},
			{Upgrade.StoneBackOnDelete, [3, 7, 15]}, {Upgrade.WoodMoveCost, [2, 5, 10]},
			{Upgrade.StoneMoveCost, [2, 5, 10]}
		};
	}

	public override void ProduceItem()
	{
		if (GameLogistics.Resources["Food"] > 0 && GameLogistics.Resources["Water"] > 0)
		{
			GameLogistics.Resources["Food"]--;
			GameLogistics.Resources["Water"]--;
			IsOpen = true;
		}
		else
		{
			IsOpen = false;
		}

	}

}
