using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Godot;
using Scripts.Constants;

public partial class MarketStall : Production
{
	private Button _button;
	public string itemToSell;
	private int price;
	public Panel Panel;
	public int MarketStash;
	public List<TradeObject> SellObjects = [];
	public List<TradeObject> BuyObjects = [];
	public Dictionary<string, int> allResources = new();
	public Dictionary<string,int> ResourcesToSell = new ();
	public Dictionary<string,int> ResourcesToBuy = new ();
	public Dictionary<string, int> PointsOnSell = new();
	public Dictionary<string, int> PointsToBuy = new();

	protected override void _Ready_instance()
	{
		ActivityIndoors = false;
		BuildingName = "Market Stall";
		BuildingDescription = "Market stall to sell resources";
		Panel = GetNode<Panel>("Panel");
		
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
		PointsOnSell = new Dictionary<string, int>()
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
			;
		Panel.GetNode<Button>("Button").Pressed += () =>
		{
			GetNode<PlaceableInfo>("PlaceableInfo").Visible = true;
			Panel.Visible = false;
		};
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
			tradeObject.Value.Text = $"({resource.Value.ToString()})";
			tradeObject.Plus.Pressed += () =>
			{
				var amountInt = int.Parse(tradeObject.Amount.Text);
				tradeObject.Amount.Text = Math.Min(amountInt + 1, resource.Value).ToString();
			};
			SellObjects.Add(tradeObject);
			ResourcesToSell.Add(resource.Key, resource.Value);
			
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
				var amountInt = int.Parse(tradeObject.Amount.Text);
				tradeObject.Amount.Text = (amountInt + 1).ToString();
			};
			BuyObjects.Add(tradeObject);
			ResourcesToBuy.Add(resource.Key, resource.Value);
		}
	}
	
	public override void _Process(double delta)
	{
		GetAllResources();
		if (Panel.Visible)
		{
			foreach (var tradeObject in SellObjects)
			{
				var resource = allResources[tradeObject.Label.Text]; //update amount of resources
				tradeObject.Value.Text = $"({resource.ToString()})";
			}
		}
	}

	public override void ProduceItem()
	{
		foreach (var resource in ResourcesToSell)
		{
			if (resource.Value > 0)
			{
				ResourcesToSell[resource.Key] -= 1;
				MarketStash += Math.Min(PointsOnSell[resource.Key], 20);
			}
		}
	}

	public void BuyResource()
	{
		List<string> remove = [];
		remove.AddRange(from resource in ResourcesToBuy where resource.Value == 0 select resource.Key);
		foreach (var resource in remove) ResourcesToBuy.Remove(resource);

		foreach (var resource in ResourcesToBuy)
		{
			if (resource.Value == 0) // doesnt want resource
			{
				remove.Add(resource.Key);
				continue;
			}
			if (PointsToBuy[resource.Key] > MarketStash) continue; //cant afford
			
			foreach (var buyObject in BuyObjects)
			{
				if(buyObject.Label.Text != resource.Key) continue;
				
				ResourcesToBuy[resource.Key] -= 1;
				MarketStash -= PointsToBuy[resource.Key];
				AddResource(resource.Key);
				return;
			}
		}
	}
	
	public void SellResource()
	{
		List<string> remove = [];
		remove.AddRange(from resourceToSell in ResourcesToSell where resourceToSell.Value == 0 select resourceToSell.Key);
		foreach (var resource in remove) ResourcesToBuy.Remove(resource);
		
		foreach (var resource in ResourcesToSell)
		{
			if (allResources[resource.Key] == 0) continue; //no resource to sell			
			foreach (var sellObject in SellObjects)
			{
				if(sellObject.Label.Text != resource.Key) continue;
				
				ResourcesToSell[resource.Key] -= 1;
				MarketStash = Math.Min(MarketStash + PointsOnSell[resource.Key], 20);
				RemoveResource(resource.Key);
				return;
			}
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
