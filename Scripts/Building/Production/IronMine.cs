using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class IronMine : Production
{
	// Called when the node enters the scene tree for the first time.
	

	protected override void _Ready_instance()
	{
		BuildingName = "Iron Mine";
		BuildingDescription = "A Mine for Iron";
		ActivityIndoors = false;
		Producing = "Iron";
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [10, 15, 20] }
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
		ProductionRate = 25;
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
		GameLogistics.Resources[RawResource.Iron]++;
	}
}
