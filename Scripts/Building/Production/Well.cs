using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class Well : Production
{
	protected override void _Ready_instance()
	{
		BuildingName = "Well";
		BuildingDescription = "Allows citizens to get water";

		Producing = GameResource.Water;
		ProductionRate = 2;
		PlayerLevel = 0;
		_timer = GetNode<Timer>("FoodTimer");
		_timer.Start();
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] }
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
		GameLogistics.Resources[GameResource.Water]++;
	}
}
