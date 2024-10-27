using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Decoration;
using Scripts.Constants;

public partial class Lantern : Decoration
{
	public int increaseHappiness = 1;

	protected override void _Ready_instance()
	{		
		BuildingName = "Lantern";
		PlayerLevel = 3;
		BuildingDescription = "A lantern lighting up the streets, making the citizens happier";
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
