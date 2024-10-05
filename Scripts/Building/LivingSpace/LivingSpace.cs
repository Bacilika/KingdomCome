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
		GameLogistics.Resources["Citizens"]-= Inhabitants;
		for (int i = People.Count-1; i > 0; i--)
		{
			var npc = People[i];
			npc.OnDelete();
		}

		
		QueueFree();
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Resources["Wood"] += Upgrades["WoodBackOnDelete"][Level];
		GameLogistics.Resources["Stone"] += Upgrades["StoneBackOnDelete"][Level];
		Shop.deleteAudio.Play();
	}
}
