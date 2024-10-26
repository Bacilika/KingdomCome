using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class StoneMine : Production
{
	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		ActivityIndoors = false;
		BuildingName = "Stone Mine";
		BuildingDescription = "Mine for Mining stone";
		Producing = RawResource.Stone;
		PlayerLevel = 2;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [2, 5, 8] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [7, 7, 10] },
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
		npc.SetDestination(new Vector2(458, -436));
		npc.AtWorkTimer.SetWaitTime(5);
		npc._move = true;
		npc.Idle = false;

	}

	public override void AtWorkTimerTimeout(Npc npc)
	{
		GatherResource(npc.Position);
		npc.SetDestination(Position);
		npc._move = true;
	}
	
	public override void ProduceItem()
	{
		GameLogistics.Resources[RawResource.Stone]++;
	}
}
