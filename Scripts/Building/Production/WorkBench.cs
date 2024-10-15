using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class WorkBench : Production
{
	
	public Dictionary<AbstractPlaceable, List<Npc>> BuildList = new ();
	protected override void _Ready_instance()
	{
		BuildingName = "WorkBench";
		BuildingDescription = "Workbench";
		PlayerLevel = 1;
		isDone = true;
		IsPlaced = true;
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

	protected override void Tick()
	{
		if (BuildList.Count > 0)
		{
			List<AbstractPlaceable> toBeRemoved = new List<AbstractPlaceable>();
			foreach (var person in People)
			{
				var busy = false;
				foreach (var building in BuildList)
				{

					if (building.Value.Contains(person)){
						if (building.Key.BuildingCounter >= 25)
						{
							toBeRemoved.Add(building.Key);
							person.SetDestination(Position);
							person._move = true;
							building.Key.isDone = true;
							building.Key.HouseSprite.SetAnimation("Level" + building.Key.Level); 
						}
						else
						{
							busy = true;
							break;
						}
					}
				}
				if (!busy)
				{
					BuildList.First().Value.Add(person);
					person.SetDestination(BuildList.First().Key.Position);
					person._move = true;
				}

				if (person.Position.DistanceTo(BuildList.First().Key.Position) < 10)
				{
					person._move = false;
					BuildList.First().Key.BuildingCounter ++;
				}
			}

			foreach (var workplace in toBeRemoved)
			{
				BuildList.Remove(workplace);
			}
			toBeRemoved.Clear();
		}
		else
		{
			foreach (var person in People){
				person.SetDestination(Position);
				person._move = true;
			}
		}
		UpdateInfo();
	}
	

	public override void AtWorkTimerTimeout(Npc npc)
	{
		npc.SetDestination(Position);
		npc._move = true;
	}
	
	
	
}
