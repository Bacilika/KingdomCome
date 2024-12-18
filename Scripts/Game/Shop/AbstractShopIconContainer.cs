using System;
using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.Building;

public abstract partial class AbstractShopIconContainer : ScrollContainer
{
    private Color _canBuy = new(1, 1, 1); //transparent
    private Color _cantAfford = new(1, 0, 0); //red
    private Color _disabled = new("#696969"); //grey

    public HBoxContainer childContainer;
    public Shop GameShop;

    public List<AbstractPlaceable> Products = [];
    public PackedScene ShopIconScene;
    public List<ShopIcon> Stock = [];


    public override void _Ready()
    {
        ShopIconScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/ShopIcon.tscn");
        childContainer = GetNode<HBoxContainer>("HBoxContainer");
        GameShop = GetParent().GetParent<Shop>();
        AddProducts();
        foreach (var product in Products)
        {
            AddChild(product);
            var shopIconControl = ShopIconScene.Instantiate<ShopIcon>();
            childContainer.AddChild(shopIconControl);
            shopIconControl.SetUp(product, GameShop);
            RemoveChild(product);
            Stock.Add(shopIconControl);
        }
        Stock.Sort((x, y) => x.Product.PlayerLevel.CompareTo(y.Product.PlayerLevel));
        foreach (var child in childContainer.GetChildren()) childContainer.RemoveChild(child);

        foreach (var item in Stock) childContainer.AddChild(item);
        if (this is Roads)
        {
            var removeRoad = ShopIconScene.Instantiate<ShopIcon>();
            childContainer.AddChild(removeRoad);
            removeRoad.AddRoadRemoval();
            removeRoad.Shop = GameShop;
            Stock.Add(removeRoad);
            
        } 
    }
    
    public abstract void AddProducts();

    public override void _Process(double delta)
    {
        UpdateStock(GameLogistics.Resources);
    }

    public void UpdateStock(Dictionary<string, int> resources)
    {
        foreach (var item in Stock)
        {
            if (item.RoadRemoval is not null)
            {
                continue;
            }
            foreach (var cost in item.Product.BuildCost){
                if (GameLogistics.ProcessedResources.ContainsKey(cost.Key))
                    resources = GameLogistics.ProcessedResources;
                else if (GameLogistics.FoodResource.ContainsKey(cost.Key)) resources = GameLogistics.FoodResource;
                if (item.Product.PlayerLevel > GameMap.Level) //too low level
                {
                    item.Icon.SelfModulate = _disabled;
                    item.Icon.Disabled = true;
                }
                else if (resources[cost.Key] < cost.Value[item.Product.Level]) //not enough resources
                {
                    item.Icon.SelfModulate = _cantAfford;
                    item.Icon.Disabled = true;
                    item.TooltipText = "Cannot afford";
                }
                else
                {
                    item.Icon.SelfModulate = _canBuy;
                    item.Icon.Disabled = false;
                }
            }
        }
    }
}