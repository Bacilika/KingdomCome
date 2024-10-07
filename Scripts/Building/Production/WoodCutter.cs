using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KingdomCome.Scripts.MapResources;
using Scripts.Constants;

public partial class WoodCutter : Production
{
	private Forest forest;
	private Dictionary<Npc, AbstractResource> _assignedWorker;
	public override void _Ready_instance()
	{
		BuildingName = "WoodCutter";
		BuildingDescription = "Place for citizen to chop wood";
		Producing = "Wood";
		PlayerLevel = 1;
		_timer = GetNode<Timer>("WoodTimer");

		_assignedWorker = new() { };
		


		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] },
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [2, 3, 4] },
			{ GameResource.Stone, [2, 3, 4] }
		};
	}

	public override void AtWork(Npc npc)
	{
		if (_assignedWorker[npc] == null)
		{
			_assignedWorker[npc] = forest.trees[Rnd.RandiRange(0, forest.trees.Count-1)];
		}
		
		npc.setDestination(_assignedWorker[npc].Position);
	}
	
	public override void ProduceItem()
	{
		GameLogistics.Resources[GameResource.Wood]++;
	}

	public override void WhenShopReady()
	{
		forest = GetParent<GameMap>().GetNode<Forest>("Forest");

	}

}
