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
            if (product is Road) Console.Write("Road");
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
    }


    public abstract void AddProducts();

    public override void _Process(double delta)
    {
    }

    public void UpdateStock(Dictionary<string, int> resources)
    {
        foreach (var item in Stock)
        foreach (var cost in item.Product.BuildCost)
            if (item.Product.PlayerLevel > GameMap.Level) //too low level
            {
                item.Icon.SelfModulate = _disabled;
                item.Icon.Disabled = true;
                item.TooltipText = $"Unlocks at level {item.Product.PlayerLevel}";
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