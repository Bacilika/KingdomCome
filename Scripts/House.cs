using Godot;
using System;

public partial class House : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _citizens;
	private const int MaxHabitants = 5;
	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		Price = 5000;
	}
	
	protected override void OnDelete()
	{
		Console.WriteLine("On delete");
		GameMenu.Citizens-= _citizens;
		QueueFree();
	}

	protected override void OnUpgrade()
	{
		Console.WriteLine("On Upgrade");
		Level++;

	}
	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
		}
		else
		{
			if (_citizens < MaxHabitants)
			{
				if (habitantGrowth.RandiRange(0, _growth) ==0)
				{
					_citizens++;
					GameMenu.Citizens++;
				}
			}
			UpdateInfo();
		}
		
	}


	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Citizens: " + _citizens;
	}
}
