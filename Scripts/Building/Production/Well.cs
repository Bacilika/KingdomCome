using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class Well : Production
{
	protected override void _Ready_instance()
	{
		BuildingName = "Well";
		BuildingDescription = "Allows citizens to get water";
		ActivityIndoors = false;
		Producing = RawResource.Water;
		PlayerLevel = 2;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [1, 2, 4] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 5, 5] },
			{ RawResource.Stone, [10, 8, 8] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
			{ RawResource.Stone, [1, 2, 3] }
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 3, 4] },
			{ RawResource.Stone, [2, 3, 4] }
		};
	}

	public override void ProduceItem()
	{
		GameLogistics.Resources[RawResource.Water]++;
	}
}
