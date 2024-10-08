using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class Road : AbstractPlaceable
{
	protected override void _Ready_instance()
	{
		BuildingName = "Road";
		BuildingDescription = "Road for citizens to walk on";
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Stone, [1, 1, 1] }
		};
	}

}
