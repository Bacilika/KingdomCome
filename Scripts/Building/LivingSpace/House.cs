using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class House : LivingSpace
{
	[Signal]
	public delegate void OnCreateNpcEventHandler(House house);

	private int _growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
	private Npc Npc;

	// Called when the node enters the scene tree for the first time.
	protected override void _Ready_instance()
	{
		PlayerLevel = 0;
		BuildingName = "House";
		BuildingDescription = "A place for citizens to live";
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxInhabitants, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] }
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
			if (Rnd.RandiRange(0, _growth) == 0)
			{
				Inhabitants++;
				GameLogistics.Resources[GameResource.Citizens]++;
				PlayAnimation();
				EmitSignal(SignalName.OnCreateNpc, this);
			}

		if (!_isPeopleInside())
		{
			GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("default");
			GetNode<AnimatedSprite2D>("HouseSprite").Pause();
			GetNode<AnimatedSprite2D>("HouseSprite").Frame = Level;
		}

		UpdateInfo();
	}

	private bool _isPeopleInside()
	{
		foreach (var person in People)
			if (Position.DistanceTo(person.Position) < 15)
			{
				GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("Level 1 people inside");
				GetNode<AnimatedSprite2D>("HouseSprite").Play();
				return true;
			}

		return false;
	}


	public void UpdateInfo()
	{
		InfoBox.UpdateInfo(GetBuildingName(),
			"Inhabitants: " + Inhabitants + "/" + Upgrades[Upgrade.MaxInhabitants][Level]);
	}
}
