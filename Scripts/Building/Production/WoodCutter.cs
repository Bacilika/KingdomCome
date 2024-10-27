using System;
using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.MapResources;
using Scripts.Constants;

public partial class WoodCutter : Production
{
	private Forest forest;
	private double time;

	protected override void _Ready_instance()
	{
		BuildingName = "WoodCutter";
		BuildingDescription = "A cottage providing tools for wood cutting.";
		Producing = "Wood";
		PlayerLevel = 1;
		_timer = GetNode<Timer>("WoodTimer");
		
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [3, 5, 7] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 5] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 3, 4] },
		};
	}
	
	public override void NpcWork(Npc npc)
	{
		npc.CurrentBuilding = npc.Work;
		npc.TargetBuilding = npc.Work;
		npc.SetDestination(forest.GetClosest(npc).GlobalPosition);
		npc.AtWorkTimer.SetWaitTime(5);
		npc._move = true;
		npc.Idle = false;

	}
	

	public override void AtWorkTimerTimeout(Npc npc)
	{
		var tree = forest.GetClosestTo(npc.Position);
		GatherResource(npc.Position);
		forest.RemoveResource(tree);
		tree.AssignedNpcs.Remove(npc);
		npc.SetDestination(Position);
		npc._move = true;
	}

	public override void ProduceItem()
	{
		
		int production = ProductionRate.RandiRange(1, 3);
		if (production == 1) GameLogistics.Resources[RawResource.Wood]++;
	}

	public override void OnParentReady()
	{
		
		try
		{
			forest = GetParent<GameMap>().GetNode<Forest>("Forest");
		}
		catch
		{
			// ignored
		}
	}
}
