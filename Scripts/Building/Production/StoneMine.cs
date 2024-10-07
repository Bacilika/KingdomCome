using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class StoneMine : Production
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		BuildingName = "Stone Mine";
		BuildingDescription = "Mine for Mining stone";
		Producing = GameResource.Stone;
		ProductionRate = 2;
		PlayerLevel = 1;
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
		GameLogistics.Resources[GameResource.Stone]++;
	}
	
	
}
