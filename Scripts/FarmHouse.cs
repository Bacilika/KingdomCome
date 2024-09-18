using Godot;
using System;

public partial class FarmHouse : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private RandomNumberGenerator foodGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _workers;
	private const int MaxWorkers = 10;
	private int _food;
	private bool _timerTimedOut = false;
	private Timer _timer; 

	private const int price = 20000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetNode<Control>("PlaceableInfo");
		InfoBox.Visible = false;
		_timer = GetNode<Timer>("FoodTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
		}

		else if (_timer.IsStopped() && _workers > 0)
		{
			_timer.Start();
		}
		else
		{
			if (_workers < MaxWorkers && GameMenu.WorkingCitizens < GameMenu.Citizens)
			{
				if (habitantGrowth.RandiRange(0, _growth) ==0)
				{
					_workers++;
					GameMenu.WorkingCitizens++;
				}
			}
			UpdateInfo();
		}
		
	}
	
	public override void OnDelete()
	{
		Console.WriteLine("On delete farmhouse");
		QueueFree();
	}

	public override void _Ready_instance()
	{
		var button = InfoBox.GetNode<Button>("InfoBox/DeleteButton");
		button.Connect(DeleteButton.SignalName.OnDelete, Callable.From(OnDelete));
	}
	public void OnFoodTimerTimeout()
	{
		_food++;
		GameMenu.Food++;
		_timer.SetWaitTime(30/(Math.Sqrt(_workers)));
	}
	
	public int GetPrice()
	{
		return price;
		
	}
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + _workers;
	}
	public void ShowInfo()
	{
		InfoBox.Visible = !InfoBox.Visible;
	}
}
