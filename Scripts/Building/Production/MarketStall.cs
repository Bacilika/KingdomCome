using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class MarketStall : Production
{
	private Button _button;
	public string itemToSell;
	private int price;
	private bool sell;
	protected ChooseWare WareBox;

	protected override void _Ready_instance()
	{
		ActivityIndoors = false;
		BuildingName = "Market Stall";
		BuildingDescription = "Market stall to sell resources";

		Producing = "Iron";
		InfoBox.Connect(PlaceableInfo.SignalName.OnChooseWare, Callable.From(OnChooseWare));
		PlayerLevel = 3;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [5, 7, 10] },
			{ RawResource.Stone, [5, 7, 10] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [1, 2, 3] },
			{ RawResource.Stone, [1, 2, 3] }
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [2, 3, 4] },
			{ RawResource.Stone, [2, 3, 4] }
		};
		WareBox = InfoBox.GetNode<ChooseWare>("ChooseWare");
		WareBox.Connect(ChooseWare.SignalName.OnSellIron, Callable.From(OnSellIron));
	}

	public override void ProduceItem()
	{
		if (sell && itemToSell != null)
			if (GameLogistics.Resources[itemToSell] > 0)
			{
				GameLogistics.Resources[RawResource.Money] += price;
				GameLogistics.Resources[itemToSell]--;
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
