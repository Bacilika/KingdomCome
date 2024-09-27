using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	
	
	protected override void OnDelete()
	{
		GameLogistics.Citizens-= Inhabitants;
		for (int i = People.Count-1; i > 0; i--)
		{
			var npc = People[i];
			npc.OnDelete();
		}

		
		QueueFree();
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Wood += Upgrades["WoodBackOnDelete"][Level];
		GameLogistics.Stone += Upgrades["StoneBackOnDelete"][Level];
		Shop.deleteAudio.Play();
	}
}
