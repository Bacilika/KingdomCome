using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class MarketStall : Production
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready_instance()
	{
		ProductionRate = 2;
		_timer = GetNode<Timer>("FoodTimer");
		_timer.Start();
		Price = 20000;
		InfoBox.Connect(PlaceableInfo.SignalName.OnChooseWare, Callable.From(OnChooseWare));
		Upgrades = new Dictionary<string, List<int>>
		{
			{Upgrade.Cost, [5000, 3000, 3000]}, {Upgrade.MaxWorkers, [5, 7, 10]},
			{Upgrade.Inhabitants, [0, 0, 0]}, {Upgrade.WoodCost, [1, 1, 1]},
			{Upgrade.StoneCost, [1, 1, 1]}, {Upgrade.MoneyBackOnDelete, [4000, 2000, 2000] },
			{Upgrade.WoodBackOnDelete, [3, 7, 15]}, {Upgrade.StoneBackOnDelete, [3, 7, 15]},
			{Upgrade.WoodMoveCost, [2, 5, 10]}, {Upgrade.StoneMoveCost, [2, 5, 10]}
		};
		var button = InfoBox.GetNode<Button>("InfoBox/ChooseWareButton");
		button.Visible = true;
	}
	
	public override void ProduceItem()
	{
		GameLogistics._money++;
	}

	public void OnChooseWare()
	{
		
	}
}
