using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Brewery : Production
{
	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		BuildingName = "Brewery";
		BuildingDescription = "Produces ale from flour";
		Producing = RawResource.Stone;
		PlayerLevel = 5;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ ProcessedResource.Plank, [10, 5, 5] },
			{ RawResource.Stone, [7, 3, 5] },
			{ RawResource.Iron, [20, 5, 3] },
			{ RawResource.Wood, [3, 2, 3] }
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
		GameLogistics.Resources[ProcessedResource.Ale]++;
	}
}
