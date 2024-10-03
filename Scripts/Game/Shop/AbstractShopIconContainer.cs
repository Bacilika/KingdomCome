using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public abstract partial class AbstractShopIconContainer : ScrollContainer
{

	public List<AbstractPlaceable> Products = [];
	public PackedScene ShopIconScene;
	
	public HBoxContainer childContainer;
	public List<ShopIcon> Stock = [];
	public Shop GameShop;

	public override void _Ready()
	{
		
		ShopIconScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/ShopIcon.tscn");
		childContainer = GetNode<HBoxContainer>("HBoxContainer");
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
			childContainer.AddChild(shopIconControl);
			shopIconControl.SetUp(product, GameShop);
			RemoveChild(product);
			Stock.Add(shopIconControl);
			Console.WriteLine(shopIconControl.Position);
		}
		
	}
	
	

	public abstract void AddProducts();
	
	public override void _Process(double delta)
	{
	}
}
