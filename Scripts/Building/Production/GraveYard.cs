using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Constants;

public partial class GraveYard : Production
{
	
	protected override void _Ready_instance()
	{
		BuildingName = "Grave yard";
		BuildingDescription = "A place where dead citizens can be places to rest.";
		Producing = "Wood";
		PlayerLevel = 2;
		_timer = GetNode<Timer>("WoodTimer");
		
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [1, 2, 3] }
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
		npc.AtWorkTimer.SetWaitTime(15);
	}
	

	public override void AtWorkTimerTimeout(Npc npc)
	{
		if (GetParent<GameMap>().DeadCitizens is not null){
			var deadnpc = GetParent<GameMap>().DeadCitizens.First();
			deadnpc.OnDelete();
			GetParent<GameMap>().DeadCitizens.Remove(npc);
		}
	}
	
}
