using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class MarketStall : Production
{
	protected ChooseWare WareBox;
	private Button _button;
	public string itemToSell = null;
	private bool sell = false;
	private int price;

	public override void _Ready_instance()
	{
		BuildingName = "Market Stall";
		BuildingDescription = "Market stall to sell resources";
		
		Producing = "Iron";
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
		WareBox = InfoBox.GetNode<ChooseWare>("ChooseWare");
		WareBox.Connect(ChooseWare.SignalName.OnSellIron, Callable.From(OnSellIron));
	}
	

	
	protected override void Tick()
	{
		
		UpdateInfo();
	}

	public override void ProduceItem()
	{
		if (sell && itemToSell != null)
		{
			if (GameLogistics.Resources[itemToSell] > 0)
			{
				GameLogistics.Resources["Money"] += price;
				GameLogistics.Resources[itemToSell]--;
			}
		}
	}

	public void OnChooseWare()
	{
		WareBox.Visible = !WareBox.Visible;
	}

	public void OnSellIron()
	{
		itemToSell = "Iron";
		sell = true;
		price = 10;
	}
	public void OnSellMeat()
	{
		itemToSell = "Food";
		sell = true;
		price = 2;
	}
	
	public void OnSellWheat()
	{
		itemToSell = "Food";
		sell = true;
		price = 3;
	}
	
	public void OnSellStone()
	{
		itemToSell = "Stone";
		sell = true;
		price = 1;
	}
	
	public void OnSellWood()
	{
		itemToSell = "Wood";
		sell = true;
		price = 1;
	}
}
