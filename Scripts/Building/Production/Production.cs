using Godot;
using System;
using System.Net;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	protected string Producing;
	private int _food;
	protected Timer _timer;
	protected int ProductionRate = 10; // 1/ProductionRate % chance to produce item by 1 each tick. 
	[Signal]
	public delegate void LookingForWorkersEventHandler(Productions production);


	
	public virtual void AtWorkTimerTimeout(Npc npc)
	{
		Console.WriteLine("Production instance");
	}

	protected override void Tick()
	{
		if ( _timer is not null && _timer.IsStopped())
		{
			_timer.Start();
		}
		UpdateInfo();
	}

	public void GatherResource()
	{
		if (GetWorkers() > 0)
		{
			ProduceItem();
			PlayAnimation();	
		}
	}

	public abstract void ProduceItem();
	public abstract override void _Ready_instance();

	public virtual void AtWork(Npc npc)
	{
		Console.WriteLine("Production AtWork");
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
		else
		{
			bool employed;
			if (GameMenu.GameMode.Text == GameMode.JobChange)
			{
				employed = npc.GetJob(this, true);
			}
			else
			{
				employed = npc.GetJob(this);
			}
			
			if (employed)
			{
				GameLogistics.Resources[GameResource.Unemployed]--;
				People.Add(npc);
			}
			
			npc.OnAtWork += AtWork;
			return true;
			
			
		}
	}

	public void RemoveWorker(Npc npc)
	{
		People.Remove(npc);
	}
	
	public void OnFoodTimerTimeout()
	{
		_food++;
		ProduceItem();
		float time = 15 - GetWorkers();
		_timer.Start(time);
	}
	
	protected override void OnDeleteInstance()
	{
	}
	
	public void UpdateInfo()
	{
		var info = $"Produces  {Producing}\nWorkers: {GetWorkers()} / {Upgrades[Upgrade.MaxWorkers][Level]}";
		InfoBox.UpdateInfo(GetBuildingName(),info);
	}
}
