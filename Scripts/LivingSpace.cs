using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	
	public override void _ReadyProduction()
	{
	}
	
	protected override void OnDelete()
	{
		GameLogistics.Citizens-= Citizens;
		QueueFree();
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Wood += Upgrades["WoodBackOnDelete"][Level];
		GameLogistics.Stone += Upgrades["StoneBackOnDelete"][Level];
		Shop.deleteAudio.Play();
	}
}
