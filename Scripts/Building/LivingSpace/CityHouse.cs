using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class CityHouse : LivingSpaces
{
	private int _growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
	private Npc Npc;
	
	[Signal]
	public delegate void OnCreateNpcEventHandler(CityHouse house);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		BuildingName = "City House";
		BuildingDescription = "Affordable house allowing for many citizens but with less comfort";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.Cost, [1000, 750, 750] }, 
			{ Upgrade.MaxInhabitants, [5, 7, 10] },
			{ Upgrade.Workers, [5, 7, 10] },
			{ Upgrade.WoodCost, [3, 5, 7] },
			{ Upgrade.StoneCost, [2, 4, 6]}, 
			{Upgrade.MoneyBackOnDelete, [750, 500, 500] },
			{Upgrade.WoodBackOnDelete, [2, 4, 5]},
			{Upgrade.StoneBackOnDelete, [1, 2, 3]}, 
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
		GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("Level 1 people inside");
		GetNode<AnimatedSprite2D>("HouseSprite").Play();
	}


	public void UpdateInfo()
	{
		InfoBox.UpdateInfo(GetBuildingName(),"Inhabitants: " + Inhabitants + "/" +Upgrades[Upgrade.MaxInhabitants][Level]);
	}
}
