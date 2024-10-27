using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class HunterLodge : Production
{
	protected override void _Ready_instance()
	{
		BuildingName = "Hunter's Lodge";
		BuildingDescription = "A station for hunters to gather and hunt meat.";
		PlayerLevel = 0;
		Producing = "Meat (food)";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [3, 5, 7] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [7, 5, 5] },
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
		GameLogistics.FoodResource[Food.Meat]++;
	}
}
