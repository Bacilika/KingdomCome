using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class FarmHouse : Production
{
	private int _food;

	protected override void _Ready_instance()
	{
		BuildingName = "Barn";
		BuildingDescription = "A Barn that produces food";
		Producing = RawResource.Food;
		_timer = GetNode<Timer>("FoodTimer");
		PlayerLevel = 5;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ ProcessedResource.Plank, [20, 10, 10] },
			{ RawResource.Stone, [10, 7, 5] },
			{ RawResource.Iron, [10, 5, 3] },
			{ RawResource.Wood, [2, 2, 3] }
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

	public override void ProduceItem()
	{
		_food++;
		GameLogistics.Resources[RawResource.Food]++;
	}
}
