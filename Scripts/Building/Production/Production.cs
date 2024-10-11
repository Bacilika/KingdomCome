using System;
using Godot;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	private int _food;
	protected Timer _timer;
	protected string Producing;
	
	
	public virtual void ProduceItem()
	{
		Console.WriteLine("ProduceItem instance");
	}

	public virtual void AtWorkTimerTimeout(Npc npc)
	{
		GatherResource(npc.Position);
		npc.AtWorkTimer.Start();
	}
	public virtual void NpcWork(Npc npc)
	{
		npc.AtWorkTimer.SetWaitTime(5);
		npc.AtWorkTimer.Start();
	}

	protected override void Tick()
	{
		UpdateInfo();
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

		bool employed;
		if (GameMenu.GameMode.Text == GameMode.JobChange)
			employed = npc.GetJob(this, true);
		else
			employed = npc.GetJob(this);

		if (employed)
		{
			GameLogistics.Resources[RawResource.Unemployed]--;
			People.Add(npc);
		}

		return true;
	}

	public void RemoveWorker(Npc npc)
	{
		People.Remove(npc);
	}

	public void UpdateInfo()
	{
		var info = $"Produces  {Producing}" +
				   $"\nWorkers: {GetWorkers()} / {Upgrades[Upgrade.MaxWorkers][Level]}";
		InfoBox.UpdateInfo(GetBuildingName(), info);
	}
}
