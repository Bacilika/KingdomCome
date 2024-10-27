using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class FlowerVine : Decoration
{
	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		BuildingName = "WheelBarrel";
		BuildingDescription = "Cute Decoration to make your citizens happier";
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
		GetNode<AnimatedSprite2D>("HouseSprite").Play();
	}
	
}
