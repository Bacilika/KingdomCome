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
		BuildingDescription = "A lantern lighting up the streets, making the citizens happier";
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
		GetNode<AnimatedSprite2D>("HouseSprite").Play();
	}
}
