using Godot;
using System;
using System.Net;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	protected int Workers;
	private int _food;
	protected Timer _timer;
	public bool HasMaxEmployees;
	protected RandomNumberGenerator Rnd = new ();
	protected int ProductionRate = 10; // 1/ProductionRate % chance to produce item by 1 each tick. 

	protected override void Tick()
	{
		if ( _timer is not null && _timer.IsStopped())
		{
			_timer.Start();
		}
	UpdateInfo();
	}

	public abstract void ProduceItem();
	public abstract override void _Ready_instance();


	[Signal]
	public delegate void LookingForWorkersEventHandler(Production production);
	

	public void EmployWorker()
	{
		if (Workers == Upgrades[Upgrade.MaxWorkers][Level])
		{
			HasMaxEmployees = true;
			return;
		}
		Workers++;
		GameLogistics.Resources["WorkingCitizens"]++;
	}
	
	public void OnFoodTimerTimeout()
	{
		_food++;
		ProduceItem();
		float time = 15 - Workers;
		_timer.Start(time);
	}
	
	protected override void OnDelete()
	{
		GameLogistics.Resources["WorkingCitizens"] -= Workers;
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Resources["Wood"] += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Resources["Stone"] += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers + "/" + Upgrades["MaxWorkers"][Level];
	}


}
