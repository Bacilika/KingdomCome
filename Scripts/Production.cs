using Godot;
using System;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	private int _food;
	protected Timer _timer;
	
	
	protected int ProductionRate = 10; // 1/ProductionRate % chance to produce item by 1 each tick. 

	
	protected override void Tick()
	{
		
		if (!HasMaxEmployees() && GameLogistics.HasUnemployedCitizens())
		{
			if ( _timer is not null && _timer.IsStopped())
			{
				_timer.Start();
			}
		}
		UpdateInfo();
	}

	public void GatherResource()
	{
		ProduceItem();
		PlayAnimation();
	}

	public abstract void ProduceItem();
	public abstract override void _Ready_instance();

	public int GetWorkers()
	{
		return People.Count;
	}


	[Signal]
	public delegate void LookingForWorkersEventHandler(Production production);

	public bool HasMaxEmployees()
	{
		return GetWorkers() >= Upgrades[Upgrade.MaxWorkers][Level];
	}

	public bool EmployWorker(Npc npc)
	{
		if (HasMaxEmployees())
		{
			Console.WriteLine("Workplace is full");
			return false;
		}
		else
		{
			GameLogistics.WorkingCitizens++;

			People.Add(npc);
			npc.GetJob(this);
			return true;
		}

	}
	
	public void OnFoodTimerTimeout()
	{
		_food++;
		GameLogistics.Food++;
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
		GameLogistics.WorkingCitizens -= GetWorkers();
		GameLogistics.Wood += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Stone += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public void UpdateInfo()
	{
		InfoBox.UpdateInfo("Workers: " + GetWorkers());
	}
	


}
