using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class SawMill : Production
{
	protected override void _Ready_instance()
	{
		BuildingName = "Bakery";
		BuildingDescription = "Produces bread from wheat";
		Producing = RawResource.Stone;
		ProductionRate = 2;
		PlayerLevel = 5;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
			{ RawResource.Stone, [1, 2, 3] }
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 3, 4] },
			{ RawResource.Stone, [2, 3, 4] }
		};
	}

	public override void ProduceItem()
	{
		GameLogistics.Resources[ProcessedResource.Plank]++;
	}
}
