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
		PlayerLevel = 0;
		BuildingName = "House";
		BuildingDescription = "A place for citizens to live";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxInhabitants, [5, 7, 10] },
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [2, 3, 4] },
			{ GameResource.Stone, [2, 3, 4] }
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
				GameLogistics.Resources[GameResource.Citizens]++;
				PlayAnimation();
				EmitSignal(SignalName.OnCreateNpc, this);
			}
		}
		UpdateInfo();
	}




	public void UpdateInfo()
	{
		InfoBox.UpdateInfo(GetBuildingName(),"Inhabitants: " + Inhabitants + "/" +Upgrades[Upgrade.MaxInhabitants][Level]);
	}
}
