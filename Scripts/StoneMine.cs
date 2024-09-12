using Godot;
using System;

public partial class StoneMine : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 100; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _workers;
	private int _stone;
	private RandomNumberGenerator stoneGrowth = new ();
	private Control _infoBox;
	private const int MaxWorkers = 5;
	private int _stoneGrowth = 500; // 1/_growth% chance to increase habitants by 1 each tick. 


	private const int price = 15000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_infoBox = GetNode<Control>("PlaceableInfo");
		_infoBox.Visible = false;
	}

	public int GetPrice()
	{
		return price;
		
	}
	
	
	
	public override void _Process(double delta)
	{
		if (!IsPlaced)
		{
			FollowMouse(); 
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
			if (stoneGrowth.RandiRange(0, _stoneGrowth) == 0)
			{
				_stone++;
				GameMenu.Stone++;
			}


			UpdateInfo();
		}
		
	}

	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) _infoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + _workers;
	}
	public void ShowInfo()
	{
		_infoBox.Visible = !_infoBox.Visible;
	}

}
