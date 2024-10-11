using System;
using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.MapResources;
using Scripts.Constants;

public partial class WoodCutter : Production
{
	private Forest forest;
	private List<Tree> _trees = [];

	protected override void _Ready_instance()
	{
		BuildingName = "WoodCutter";
		BuildingDescription = "Place for citizen to chop wood";
		Producing = "Wood";
		PlayerLevel = 1;
		_timer = GetNode<Timer>("WoodTimer");
		
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
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
	}
	
	
	public override void OnNpcReachWork(Node2D node)
	{
		var npc = node as Npc;
		if (npc.WorkTimer.IsStopped() && npc.PlaceablePosition != this)
		{
			return;
		}
		_assignedWorker.TryAdd(npc, null);
		if (_assignedWorker[npc] == null)
			_assignedWorker[npc] = _trees[Rnd.RandiRange(0, Upgrades[Upgrade.MaxWorkers][Level] + 3)];

		if (_assignedWorker[npc] != null) npc.SetDestination(_assignedWorker[npc].GlobalPosition);
		npc.AtWorkTimer.SetWaitTime(10 + Position.DistanceTo(_assignedWorker[npc].GlobalPosition) / 100);
		if (npc.AtWorkTimer.IsStopped())
		{
			npc.AtWorkTimer.Start();
		}
	}

	public override void AtWorkTimerTimeout(Npc npc)
	{
		npc.SetDestination(Position);
	}

	public override void ProduceItem()
	{
		GameLogistics.Resources[RawResource.Wood]++;
	}

	public override void OnParentReady()
	{
		try
		{
			forest = GetParent<GameMap>().GetNode<Forest>("Forest");
			foreach (var tree in forest.trees)
			{
				_trees.Add(tree);
			}
			_trees.Sort((x, y) => x.GlobalPosition.DistanceTo(Position).CompareTo(y.GlobalPosition.DistanceTo(Position)));
		}
		catch (Exception _)
		{
			// ignored
		}
	}
}
