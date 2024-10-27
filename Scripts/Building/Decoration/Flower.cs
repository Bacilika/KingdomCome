using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class Flower : Decoration
{

	protected override void _Ready_instance()
	{
		BuildingName = "Flower";
		BuildingDescription = "A beautiful flower making your citizens happier";
		Upgrades = new Dictionary<string, List<int>>();
		PlayerLevel = 0;
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1,1,1] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1,1,1] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1,1,1] },
		};
	}
}
