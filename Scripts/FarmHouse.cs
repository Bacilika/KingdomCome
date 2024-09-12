using Godot;
using System;

public partial class FarmHouse : AbstractPlaceable
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 50; // 1/_growth% chance to increase habitants by 1 each tick. 
	private int _habitants;
	private Control _infoBox;
	private const int MaxHabitants = 10;
	private int _food; 

	private const int price = 20000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_infoBox = GetNode<Control>("PlaceableInfo");
		_infoBox.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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

			_food++;
			GameMenu.Food++;
			UpdateInfo();
		}
		
	}
	
	public int GetPrice()
	{
		return price;
		
	}
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) _infoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + _habitants;
	}
	public void ShowInfo()
	{
		_infoBox.Visible = !_infoBox.Visible;
	}
}
