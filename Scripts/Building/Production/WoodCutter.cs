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
		
		_assignedWorker.TryAdd(npc, null);
		if (_assignedWorker[npc] == null)
		{
			_assignedWorker[npc] = forest.trees[Rnd.RandiRange(0, forest.trees.Count-1)];
		}

		if (_assignedWorker[npc] != null)
		{
			npc.SetDestination(_assignedWorker[npc].GlobalPosition);
		}
		npc.AtWorkTimer.SetWaitTime(10 + Position.DistanceTo(_assignedWorker[npc].GlobalPosition)/100);
		npc.AtWorkTimer.Start();
		Console.WriteLine(npc.AtWorkTimer.TimeLeft);
	}

	public override void AtWorkTimerTimeout(Npc npc)
	{
		npc.SetDestination(Position);
	}
	
	public override void ProduceItem()
	{
		GameLogistics.Resources[GameResource.Wood]++;
	}

	public override void WhenShopReady()
	{

		try
		{
			forest = GetParent<GameMap>().GetNode<Forest>("Forest");
		}
		catch (Exception _){}

	}

}
