using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class CityHouse : LivingSpace
{
	private int _growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
	private Npc Npc;

	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		PlayerLevel = 4;
		BuildingName = "City house";
		BuildingDescription = "A big living space with room for many people.";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxInhabitants, [10, 15, 20] }
		};
		
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] },
			{ ProcessedResource.Plank, [3, 7, 10] },
			{ RawResource.Iron, [3, 7, 10] }
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
