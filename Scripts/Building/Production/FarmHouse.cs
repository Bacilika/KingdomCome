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
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] },
			{ RawResource.Iron, [3, 5, 7] }
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
