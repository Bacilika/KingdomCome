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
	
	public override void NpcWork(Npc npc)
	{
		npc.CurrentBuilding = npc.Work;
		npc.TargetBuilding = npc.Work;
		npc.SetDestination(forest.GetClosest(npc).GlobalPosition);
		npc.AtWorkTimer.SetWaitTime(5);
		npc._move = true;
		
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
		GameLogistics.Resources[RawResource.Wood]++;
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
