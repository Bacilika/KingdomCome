using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class Brush : AbstractPlaceable
{
	public int increaseHappiness = 1;

	protected override void _Ready_instance()
	{
		BuildingName = "Bush";
		BuildingDescription = "A beautiful bush making your citizens happier";
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
