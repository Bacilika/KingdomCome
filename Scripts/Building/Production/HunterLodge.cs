using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class HunterLodge : Production
{
	protected override void _Ready_instance()
	{
		BuildingName = "Hunter's Lodge";
		BuildingDescription = "A station for hunters to gather and hunt. Produces meat.";
		PlayerLevel = 0;
		Producing = "Food";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [2, 5, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 3, 4] },
		};
	}

	public override void ProduceItem()
	{
		int production = ProductionRate.RandiRange(1, 5);
		if (production == 1) GameLogistics.FoodResource[Food.Meat]++;
	}
}
