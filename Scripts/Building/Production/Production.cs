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
				GameLogistics.Resources["UnEmployed"]--;
				People.Add(npc);
			}
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
	
	protected override void OnDelete()
	{
		for (int i = People.Count-1; i > 0; i--)
		{
			var npc = People[i];
			npc.OnDelete();
		}
		GameLogistics.Resources["UnEmployed"] += GetWorkers();
		GameLogistics.Resources["Wood"] += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Resources["Stone"] += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public void UpdateInfo()
	{
		var info = $"Produces  {Producing}\nWorkers: {GetWorkers()} / {Upgrades[Upgrade.MaxWorkers][Level]}";
		InfoBox.UpdateInfo(GetBuildingName(),info);
	}
}
