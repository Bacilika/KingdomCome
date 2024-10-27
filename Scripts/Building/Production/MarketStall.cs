using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Godot;
using Scripts.Constants;

public partial class MarketStall : Production
{
	private Button _button;
	public string itemToSell;
	private int price;
	public Panel Panel;
	public int MarketStash;
	public Dictionary<string,TradeObject> SellObjects = [];
	public Dictionary<string,TradeObject> BuyObjects = [];
	public Dictionary<string, int> allResources = new();
	public Dictionary<string,int> ResourcesToSell = new ();
	public Dictionary<string,int> ResourcesToBuy = new ();
	public Dictionary<string, int> PointsOnSell = new();
	public Dictionary<string, int> PointsToBuy = new();
	public Label MarketStashLabel;
	protected override void _Ready_instance()
	{
		ActivityIndoors = false;
		BuildingName = "Market Stall";
		BuildingDescription = "Market stall to sell resources";
		Panel = GetNode<Panel>("Panel");
		MarketStashLabel = Panel.GetNode<Label>("Label/MarketStash");
		
		
		PlayerLevel = 0;
		Upgrades = new Dictionary<string, List<int>>
		{
			{ Upgrade.MaxWorkers, [5, 7, 10] }
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ RawResource.Wood, [0, 7, 10] },
			{ RawResource.Stone, [0, 7, 10] }
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ ProcessedResource.Plank, [1, 2, 3] },
			{ RawResource.Stone, [1, 2, 3] }
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ ProcessedResource.Plank, [2, 3, 4] },
			{ RawResource.Stone, [2, 3, 4] }
		};

		PointsOnSell = new Dictionary<string, int>()
		{
			{ RawResource.Wood, 1 },
			{ RawResource.Stone, 2 },
			{ RawResource.Iron, 3 },
			{ ProcessedResource.Plank, 3 },
			{ ProcessedResource.Ale, 3 },
			{ ProcessedResource.IronIngot, 3 },
			{ ProcessedResource.Flour, 3 },
			{ Food.Meat, 3 },
			{ Food.Crops, 3 },
			{ Food.Bread, 3 },
		};
		PointsToBuy = new Dictionary<string, int>()
		{
			{ RawResource.Wood, 3 },
			{ RawResource.Stone, 4 },
			{ RawResource.Iron, 6 },
			{ ProcessedResource.Plank, 5 },
			{ ProcessedResource.Ale, 6 },
			{ ProcessedResource.IronIngot, 7 },
			{ ProcessedResource.Flour, 5 },
			{ Food.Meat, 5 },
			{ Food.Crops, 5 },
			{ Food.Bread, 5 },
		};


		if (GetParent() is not GameMap) return;
		foreach (var resource in GameLogistics.Resources)
		{
			if(resource.Key == RawResource.Happiness) continue;
			allResources.Add(resource.Key, resource.Value);
		}

		foreach (var resource in GameLogistics.ProcessedResources)
		{
			allResources.Add(resource.Key, resource.Value);
		}

		foreach (var resource in GameLogistics.FoodResource)
		{
			allResources.Add(resource.Key, resource.Value);
		}
			
		SetUpSell();
		SetUpBuy();
		var switchButton = GetNode<PlaceableInfo>("PlaceableInfo").GetNode<Button>("SwitchMode");
		switchButton.MouseEntered += () => GetNode<PlaceableInfo>("PlaceableInfo").Focused = true;
		switchButton.MouseExited += () => GetNode<PlaceableInfo>("PlaceableInfo").Focused = false;
		GetNode<PlaceableInfo>("PlaceableInfo").GetNode<Button>("SwitchMode").Pressed += () =>
		{
			GetNode<PlaceableInfo>("PlaceableInfo").Visible = false;
			Panel.Visible = true;
		};
		Panel.GetNode<Button>("Button").Pressed += () =>
		{
			GetNode<PlaceableInfo>("PlaceableInfo").Visible = true;
			Panel.Visible = false;
		};
		Panel.GetNode<Button>("Exit").Pressed += () => Panel.Visible = false;
	}

	public void GetAllResources()
	{
		foreach (var resource in GameLogistics.Resources)
		{
			if(resource.Key == RawResource.Happiness) continue;
			allResources[resource.Key] = resource.Value;
		}

		foreach (var resource in GameLogistics.ProcessedResources)
		{
			allResources[resource.Key] = resource.Value;
		}

		foreach (var resource in GameLogistics.FoodResource)
		{
			allResources[resource.Key] = resource.Value;
		}
	}

	public void SetUpSell()
	{
		PackedScene tradeObjectScene = ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/TradeObject.tscn");
		foreach (var resource in allResources)
		{
			var tradeObject = tradeObjectScene.Instantiate<TradeObject>();
			Panel.GetNode<VBoxContainer>("SellPanel/sell/VBoxContainer").AddChild(tradeObject);
			tradeObject.Label.Text = resource.Key;
			tradeObject.ResourcesOwned.Text = $"({resource.Value.ToString()})";
			tradeObject.Plus.Pressed += () =>
			{
				IncreaseAmountToTrade(resource.Key,SellObjects);
			};
			tradeObject.Minus.Pressed += () =>
			{
				DecreaseAmountToTrade(resource.Key,SellObjects);
			};
			SellObjects.Add(resource.Key,tradeObject);
		}
	}
	public void SetUpBuy()
	{
		PackedScene tradeObjectScene = ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/TradeObject.tscn");
		foreach (var resource in allResources)
		{
			var tradeObject = tradeObjectScene.Instantiate<TradeObject>();
			Panel.GetNode<VBoxContainer>("BuyPanel/buy/VBoxContainer").AddChild(tradeObject);
			tradeObject.Label.Text = resource.Key;
			tradeObject.Plus.Pressed += () =>
			{
				IncreaseAmountToTrade(resource.Key,BuyObjects);

			};
			tradeObject.Minus.Pressed += () =>
			{
				DecreaseAmountToTrade(resource.Key,BuyObjects);
			};
			BuyObjects.Add(resource.Key,tradeObject);
		}
	}

	private void DecreaseAmountToTrade(string key, Dictionary<string, TradeObject> objects)
	{
		var tradeObject = objects[key];
		var amountInt = int.Parse(tradeObject.AmountToTrade.Text) -1;
		if (amountInt == 0)
		{
			objects.Remove(key);
		}
		//buying
		if (objects == BuyObjects)
		{
			tradeObject.AmountToTrade.Text = amountInt.ToString();
			ResourcesToBuy[key] = amountInt;
		}
		//selling
		else
		{
			tradeObject.AmountToTrade.Text = Math.Min(amountInt, allResources[key]).ToString();
			ResourcesToSell[key] = amountInt;
		}
	}
	
	private void IncreaseAmountToTrade(string key, Dictionary<string, TradeObject> objects)
	{
		var tradeObject = objects[key];
		var amountInt = int.Parse(tradeObject.AmountToTrade.Text) +1 ;
		
		//buying
		if (objects == BuyObjects)
		{
			tradeObject.AmountToTrade.Text = amountInt.ToString();
			ResourcesToBuy[key] = amountInt;
		}
		//selling
		else
		{
			tradeObject.AmountToTrade.Text = Math.Min(amountInt, allResources[key]).ToString();
			ResourcesToSell[key] = amountInt;
		}
	}

	public override void NpcWork(Npc npc)
	{
		_= Trade( npc);
	}

	public async Task Trade(Npc npc)
	{
		await Task.Delay(1500);
		SellResource();
		BuyResource();
		if (npc.AtWork)
		{
			_= Trade(npc);
		}
		
	}
	
	public override void _Process(double delta)
	{
		GetAllResources();
		if (Panel.Visible)
		{
			foreach (var tradeObject in SellObjects)
			{
				var resource = allResources[tradeObject.Value.Label.Text]; //update amount of resources
				tradeObject.Value.ResourcesOwned.Text = $"({resource.ToString()})";
			}
		}
		MarketStashLabel.Text = $"{MarketStash}/20";
	}

	public void BuyResource()
	{
		
		foreach (var resource in ResourcesToBuy)
		{
			if (PointsToBuy[resource.Key] > MarketStash) continue; //cant afford
			MarketStash -= PointsToBuy[resource.Key];
			AddResource(resource.Key);
			DecreaseAmountToTrade(resource.Key, BuyObjects);
			return;
		}
	}
	
	public void SellResource()
	{
		foreach (var resource in ResourcesToSell)
		{
			if (allResources[resource.Key] == 0) continue; //no resource to sell	
			
			MarketStash = Math.Min(MarketStash + PointsOnSell[resource.Key], 20); //add to stash
			RemoveResource(resource.Key);
			DecreaseAmountToTrade(resource.Key, SellObjects);
			return;
		}
	}

	public void AddResource(string resource)
	{
		if (GameLogistics.Resources.ContainsKey(resource))
		{
			GameLogistics.Resources[resource] += 1;
		}
		else if (GameLogistics.ProcessedResources.ContainsKey(resource))
		{
			GameLogistics.ProcessedResources[resource] += 1;
		}
		else if (GameLogistics.FoodResource.ContainsKey(resource))
		{
			GameLogistics.FoodResource[resource] += 1;
		}
	}
	public void RemoveResource(string resource)
	{
		if (GameLogistics.Resources.ContainsKey(resource))
		{
			GameLogistics.Resources[resource] -= 1;

		}
		else if (GameLogistics.ProcessedResources.ContainsKey(resource))
		{
			GameLogistics.ProcessedResources[resource] -= 1;
		}
		else if (GameLogistics.FoodResource.ContainsKey(resource))
		{
			GameLogistics.FoodResource[resource] -= 1;
		}
	}
}
