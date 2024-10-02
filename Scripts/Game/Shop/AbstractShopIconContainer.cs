using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public abstract partial class AbstractShopIconContainer : Button
{

	public List<AbstractPlaceable> Products = [];
	public PackedScene ShopIconScene;
	public BoxContainer ShopIconContainer;
	public List<ShopIcon> Stock = [];
	public Shop GameShop;

	public override void _Ready()
	{
		
		ShopIconScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/ShopIcon.tscn");
		ShopIconContainer = GetNode<BoxContainer>("ShopItemNode");
		GameShop = GetParent().GetParent<Shop>();
		
		AddProducts();
		foreach (var product in Products)
		{
			if (product is Road)
			{
				Console.Write("Road");
			}
			AddChild(product);
			var shopIconControl = ShopIconScene.Instantiate<ShopIcon>();
			ShopIconContainer.AddChild(shopIconControl);
			shopIconControl.SetUp(product, GameShop);
			RemoveChild(product);
			Stock.Add(shopIconControl);
		}
	}
	
	

	public abstract void AddProducts();
	
	public override void _Process(double delta)
	{
	}

	public void HideStock()
	{
		ShopIconContainer.Visible = false;
	}
	public void ShowStock()
	{
		ShopIconContainer.Visible = true;
		foreach (var item in Stock)
		{
			if (GameMap.Level >= item.Product.PlayerLevel)
			{
				
			}
		}
	}
}
