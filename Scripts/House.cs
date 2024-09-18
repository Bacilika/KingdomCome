using Godot;
using System;

public partial class House : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _habitants;
	private const int MaxHabitants = 5;
	

	private const int price = 5000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		var button = InfoBox.GetNode<Button>("InfoBox/DeleteButton");
		button.Connect(DeleteButton.SignalName.OnDelete, Callable.From(OnDelete));
	}

	public int GetPrice()
	{
		return price;
		
	}
	public override void OnDelete()
	{
		Console.WriteLine("On delete");
		GameMenu.Citizens-= _habitants;
		QueueFree();
	}
	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
		}
		else
		{
			if (_habitants < MaxHabitants)
			{
				if (habitantGrowth.RandiRange(0, _growth) ==0)
				{
					_habitants++;
					GameMenu.Citizens++;
				}
			}

			UpdateInfo();
		}
		
	}

	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Habitants: " + _habitants;
	}
	public void ShowInfo()
	{
		InfoBox.Visible = !InfoBox.Visible;
	}

}
