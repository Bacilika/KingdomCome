using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;

public partial class House : LivingSpaces
{
	private RandomNumberGenerator habitantGrowth = new ();
	private int _growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
	private Npc Npc;	
	
	[Signal]
	public delegate void OnCreateNpcEventHandler(House house);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		Price = 5000;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ "Cost", [5000, 3000, 3000] }, 
			{ "MaxInhabitants", [5, 7, 10] },
			{ "Workers", [5, 7, 10] },
			{ "WoodCost", [5, 7, 10] },
			{ "StoneCost", [5, 7, 10]}, 
			{"MoneyBackOnDelete", [4000, 2000, 2000] },
			{"WoodBackOnDelete", [3, 7, 15]},
			{"StoneBackOnDelete", [3, 7, 15]}, 
			{"WoodMoveCost", [2, 5, 10]}, 
			{"StoneMoveCost", [2, 5, 10]}
		};
	}

	public Npc GetNpc()
	{
		return Npc;
	}
	

	
	protected override void Tick()
	{
		if (Citizens < Upgrades["MaxInhabitants"][Level])
		{
			if (habitantGrowth.RandiRange(0, _growth) ==0)
			{
				Citizens++;
				GameLogistics.Citizens++;
				EmitSignal(SignalName.OnCreateNpc, this);
			}
		}
		UpdateInfo();
	}


	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Citizens: " + Citizens + "/" +Upgrades["MaxInhabitants"][Level] ;
	}


}
