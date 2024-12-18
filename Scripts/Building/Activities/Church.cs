using System.Collections.Generic;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class Church : AbstractActivity
{
	protected override void _Ready_instance()
	{
		PlayerLevel = 3;
		BuildingName = "Church";
		BuildingDescription = "The holy church provides a good place for citizens to " +
							  "reflect and pray.";

		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [2, 3, 4] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] }, 
			{ ProcessedResource.Plank, [5, 7, 10] },
			{ RawResource.Iron, [0, 5, 10] }
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
}
