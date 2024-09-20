using Godot;
using System;
using System.Collections.Generic;

public partial class WoodCutter : AbstractPlaceable{
	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private bool _timerTimedOut = false;
	private Timer _timer; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		_timer = GetNode<Timer>("WoodTimer");
		Price = 20000;
		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [0,0,0]}, {"WoodCost", [5, 10, 20]},
			{"StoneCost", [5, 10, 20]}, {"MoneyBackOnDelete", [4000, 2000, 2000] }
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
		}

		else if (_timer.IsStopped() && Workers > 0) 
		{
			_timer.Start();
		}
		else
		{
			if (Workers < Upgrades["MaxWorkers"][Level] && GameMenu.WorkingCitizens < GameMenu.Citizens)
			{
				if (habitantGrowth.RandiRange(0, _growth) ==0)
				{
					Workers++;
					GameMenu.WorkingCitizens++;
				}
			}
			UpdateInfo();
		}
	}
	
	protected override void OnDelete()
	{
		Console.WriteLine("On delete farmhouse");
		GameMenu.WorkingCitizens -= Workers;
		QueueFree();
		GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];

	}


	public void OnWoodTimerTimeout()
	{
		Console.WriteLine("Wood time out");
		//Wood++;
		//GameMenu.Wood++;
		float time = 15 - Workers;
		_timer.Start(time);
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		//textLabel.Text = "Wood: " + Wood;
	}
}
