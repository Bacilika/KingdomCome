using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Windmill : Production
{
	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		BuildingName = "Windmill";
		BuildingDescription = "Procudes flour from wheat";
		Producing = ProcessedResource.Flour;
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] },
			{ ProcessedResource.Plank, [5, 7, 10] },
			{ RawResource.Iron, [2, 5, 8] }
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
		GameLogistics.ProcessedResources[ProcessedResource.Flour]++;
		GameLogistics.Resources[RawResource.Wheat]--;
	}
}
