using Godot;
using System;
using System.Collections.Generic;

public partial class FarmHouse : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private const int MaxWorkers = 10;
	private int _food;
	private bool _timerTimedOut = false;
	private Timer _timer; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		_timer = GetNode<Timer>("FoodTimer");
		Price = 20000;
		Upgrades = new Dictionary<String, List<int>>
		{
			{"Cost", [5000, 3000, 3000]}, {"MaxWorkers", [5, 7, 10]}, {"Inhabitants", [5, 7, 10]}, {"WoodCost", [0, 0, 0]},
			{"StoneCost", [0, 0, 0]}, {"MoneyBackOnDelete", [4000, 2000, 2000] }
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


	public void OnFoodTimerTimeout()
	{
		Console.WriteLine("Food time out");
		_food++;
		GameMenu.Food++;
		float time = 15 - Workers;
		_timer.Start(time);
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers;
	}
}
