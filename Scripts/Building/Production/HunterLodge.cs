using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class HunterLodge : Production
{
	public override void _Ready_instance()
	{
		BuildingName = "Hunter's Lodge";
		BuildingDescription = "A station for hunters to gather and hunt. Produces meat.";
		
		ProductionRate = 2;
		_timer = GetNode<Timer>("FoodTimer");
		_timer.Start();
		PlayerLevel = 0;
		Producing = "Food";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] },
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [2, 3, 4] },
			{ GameResource.Stone, [2, 3, 4] }
		};
	}
	
	public override void ProduceItem()
	{
		GameLogistics.Resources[GameResource.Food]++;
	}

	
	
}
