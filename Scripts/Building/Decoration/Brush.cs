using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class Brush : Decoration
{

	protected override void _Ready_instance()
	{
		BuildingName = "Bush";
		BuildingDescription = "A beautiful bush making your citizens happier";
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>();
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1 ] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [0] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [0] },
		};
	}
}
