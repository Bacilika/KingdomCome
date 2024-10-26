using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class House : LivingSpace
{
	
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
			{ RawResource.Wood, [10, 5, 5] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 3, 4] },
		};
	}

	public Npc GetNpc()
	{
		return Npc;
	}

	protected override void DoAction()
	{
		if (!_isPeopleInside())
		{
			GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("Level" + Level);
			GetNode<AnimatedSprite2D>("HouseSprite").Pause();
		}
	}

	private bool _isPeopleInside()
	{
		foreach (var person in People)
			if (Position.DistanceTo(person.Position) < 15)
			{
				GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("AnimationLevel" + Level);
				GetNode<AnimatedSprite2D>("HouseSprite").Play();
				return true;
			}

		return false;
	}
}
