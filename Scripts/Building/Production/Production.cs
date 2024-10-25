using System;
using Godot;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	private int _food;
	protected Timer _timer;
	protected string Producing;
	protected int ProductionRate = 10; // 1/ProductionRate % chance to produce item by 1 each tick. 
	private bool isOn = true;

	
	protected override void Tick()
	{
		UpdateInfo();
	}
	protected override void OnDelete()
	{
		OnDeleteInstance();
		for (var i = People.Count -1; i > -1; i--)
		{
			var npc = People[i];
			npc.OnWorkDelete();
			RemoveWorker(npc);
		}
		foreach (var cost in DeleteCost) GameLogistics.Resources[cost.Key] += cost.Value[Level];
		Shop.deleteAudio.Play();
		foreach (var production in GameMap._placedProduction)
		{
			if (production == this)
			{
				GameMap._placedProduction.Remove(production);
				break;
			}
		}
		QueueFree();
	}

	public virtual void OnNpcReachWork(Node2D npc)
	{
		//Console.WriteLine("OnNpcReachWork");
	}

	public virtual void SpaceOutWorkers()
	{
		
	}

	public virtual void ProduceItem()
	{
		//Console.WriteLine("ProduceItem instance");
	}

	public virtual void AtWorkTimerTimeout(Npc npc)
	{
		GatherResource(npc.Position);
		npc.AtWorkTimer.Start();
	}
	public virtual void NpcWork(Npc npc)
	{
		npc.Idle = false;
		npc.AtWorkTimer.SetWaitTime(5);
		npc.AtWorkTimer.Start();
	}



	protected override void TurnOffBuilding()
	{
		if (isOn)
		{
			isOn = false;
			int npcWorking = People.Count-1;
			for (var i = People.Count -1; i > -1; i--)
			{
				var npc = People[i];
				npc.OnWorkDelete();
				RemoveWorker(npc);
			}
			//Remove from _placedProduction so NPCs can't get job there. 

		}
		else
		{
			isOn = true;
			GameMap._placedProduction.Add(this);
		}
	}
	
	
	public void GatherResource(Vector2 pos)
	{
		ProduceItem();
		PlayAnimation(pos);
	}
	public int GetWorkers()
	{
		return People.Count;
	}

	public bool HasMaxEmployees()
	{
		return GetWorkers() >= Upgrades[Upgrade.MaxWorkers][Level];
	}

	public bool EmployWorker(Npc npc)
	{
		if (HasMaxEmployees())
		{
			return false;
		}

		if (npc.GetJob(this))
		{
			People.Add(npc);
		}
		return true;
	}

	public void RemoveWorker(Npc npc)
	{
		People.Remove(npc);
	}

	public virtual void UpdateInfo()
	{
		InfoBox.UpdateInfo(GetBuildingName(), $"Produces  {Producing}\nWorkers: {GetWorkers()} / {Upgrades[Upgrade.MaxWorkers][Level]}");
	}
}
