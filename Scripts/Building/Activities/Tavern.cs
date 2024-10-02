using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Tavern : AbstractPlaceable
{
	
	protected override void Tick()
	{

	}
	
	protected override void OnDelete()
	{
		GameLogistics.Resources["Wood"] += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Resources["Stone"] += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public override void _Ready_instance()
	{
		Console.WriteLine("Church instantiated");
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
}
