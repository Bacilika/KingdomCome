using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class WorkBench : Production
{
	
	public Dictionary<AbstractPlaceable, List<Npc>> BuildList = [];
	protected override void _Ready_instance()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Animation");
		_animatedSprite.Animation = "ExclamationPoint";
		_animatedSprite.Play();
		BuildingName = "WorkBench";
		BuildingDescription = "All Npcs assigned to the workbench will help build";
		PlayerLevel = 1;
		isDone = true;
		IsPlaced = true;
		ActivityIndoors = false;
		
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
		RemoveFinishedBuildings();
		
		UpdateInfo();
		if (BuildList.Count == 0)
		{
			foreach (var person in CurrentPeople)
			{
				person.Idle = true;
			}
			return;
		}
		
		foreach (var person in People)
		{
			if (!person.AtWork) continue;
			var result = BuildList.FirstOrDefault(x => x.Value.Contains(person));
			if (!result.Equals(default(KeyValuePair<AbstractPlaceable,List<Npc>>)) ) //if person is busy
			{
				DoBuildAction(person, result.Key);
				continue;
			}
			var firstEntry = BuildList.First();
			firstEntry.Value.Add(person);
			person.SetDestination(firstEntry.Key.Position);
			
		}
	}

	private void RemoveFinishedBuildings()
	{
		List<AbstractPlaceable> toBeRemoved = [];
		foreach (var entry in BuildList.Where(entry => entry.Key.BuildingCounter >= 25))
		{
			toBeRemoved.Add(entry.Key);
			entry.Key.isDone = true;
			entry.Key.HouseSprite.SetAnimation("Level" + entry.Key.Level); 
			entry.Key.BuildingProgressBar.Visible = false;
			foreach (var person in entry.Value)
			{
				person.SetDestination(Position);
			}
		}

		foreach (var building in toBeRemoved)
		{
			BuildList.Remove(building);
		}
		toBeRemoved.Clear();
	}

	private void DoBuildAction(Npc person, AbstractPlaceable building)
	{
		if (!person.AtWork) return;
		if (person.Position.DistanceTo(building.Position) < 10)
		{
			person._move = false;
			person.Idle = false;
			building.BuildingCounter ++;
			building.BuildingProgressBar.Value += 1;
		}
		else
		{
			person.SetDestination(building.Position);
		}
	}

	public override void SpaceOutWorkers()
	{
		
		var amount = People.Count;
		if (amount == 0)
		{
			return;
		}
		var positions = 360 / amount;
		var radius = ((CircleShape2D)_hitbox.Shape).Radius;
		for (var i = 0; i < CurrentPeople.Count; i++)
		{
			var person = CurrentPeople[i];
			if (person._navigation.IsTargetReached())
			{
				continue;
			}

			var circlePos = new Vector2(radius * (float)Math.Cos(positions * i),
				radius * (float)Math.Sin(positions * i));
			person.SetDestination( Position + circlePos);
		}
	}
	public override void OnBuildingPressed()
	{
		GetNode<AnimatedSprite2D>("Animation").Visible = false;
		_animatedSprite.Stop();

	}

	public override void NpcWork(Npc npc)
	{
	}
	
	public override void UpdateInfo()
	{
		var info = $"All Npcs assigned to the workbench will build the buildings" +
				   $"\nWorkers: {GetWorkers()}";
		
		InfoBox.UpdateInfo(GetBuildingName(), info);
	}
}
