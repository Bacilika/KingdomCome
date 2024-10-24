using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class Flowerbed : Decoration
{
	protected override void _Ready_instance()
	{
		BuildingName = "Flowerbush";
		BuildingDescription = "A beautiful flowerbush making your citizens happier";
		PlayerLevel = 1;
		Upgrades = new Dictionary<string, List<int>>();
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
}
