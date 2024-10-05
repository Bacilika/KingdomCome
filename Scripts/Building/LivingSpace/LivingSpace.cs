using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();

	public void MoveIntoHouse(Npc npc)
	{
		People.Add(npc);
		var npcPortrait = InfoBox.CitizenPortrait.Instantiate<CitizenPortraitButton>();
		npcPortrait.npc = npc;
		InfoBox.PortraitContainer.AddChild(npcPortrait);
		GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("Level 1 people inside");
		GetNode<AnimatedSprite2D>("HouseSprite").Play();
	}

	protected override void OnDelete()
	{
	}


	protected override void OnDeleteInstance()
	{
	}
}
