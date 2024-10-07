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
		Producing = GameResource.Food;
		_timer = GetNode<Timer>("FoodTimer");
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] },
			{ GameResource.Iron, [5, 7, 10] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] }
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [2, 3, 4] },
			{ GameResource.Stone, [2, 3, 4] }
		};
	}


	public override void ProduceItem()
	{
		_food++;
		GameLogistics.Resources[GameResource.Food]++;
	}
}
