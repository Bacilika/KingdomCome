using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using Scripts.Constants;

public partial class House : LivingSpaces
{
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
			{ Upgrade.Cost, [5000, 3000, 3000] }, 
			{ Upgrade.MaxInhabitants, [5, 7, 10] },
			{ Upgrade.Workers, [5, 7, 10] },
			{ Upgrade.WoodCost, [5, 7, 10] },
			{ Upgrade.StoneCost, [5, 7, 10]}, 
			{Upgrade.MoneyBackOnDelete, [4000, 2000, 2000] },
			{Upgrade.WoodBackOnDelete, [3, 7, 15]},
			{Upgrade.StoneBackOnDelete, [3, 7, 15]}, 
			{Upgrade.WoodMoveCost, [2, 5, 10]}, 
			{Upgrade.StoneMoveCost, [2, 5, 10]}
		};
	}

	public Npc GetNpc()
	{
		return Npc;
	}
	

	
	protected override void Tick()
	{
		if (Inhabitants < Upgrades[Upgrade.MaxInhabitants][Level])
		{
			if (Rnd.RandiRange(0, _growth) == 0)
			{
				Inhabitants++;
				GameLogistics.Resources["Citizens"]++;
				PlayAnimation();
				EmitSignal(SignalName.OnCreateNpc, this);
			}
		}
		UpdateInfo();
	}

	public void MoveIntoHouse(Npc npc)
	{
		People.Add(npc);
		var npcPortrait = InfoBox.CitizenPortrait.Instantiate<CitizenPortraitButton>();
		npcPortrait.npc = npc;
		InfoBox.PortraitContainer.AddChild(npcPortrait);
	}


	public void UpdateInfo()
	{
		InfoBox.UpdateInfo("Inhabitants: " + Inhabitants + "/" +Upgrades[Upgrade.MaxInhabitants][Level], this.GetType().Name);
	}
}
