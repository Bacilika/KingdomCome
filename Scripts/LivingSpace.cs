using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	
	
	protected override void OnDelete()
	{
		GameLogistics.Resources["Citizens"]-= Citizens;
		QueueFree();
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Resources["Wood"] += Upgrades["WoodBackOnDelete"][Level];
		GameLogistics.Resources["Stone"] += Upgrades["StoneBackOnDelete"][Level];
		Shop.deleteAudio.Play();
	}
}
