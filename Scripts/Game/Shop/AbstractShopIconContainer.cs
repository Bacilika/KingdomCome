using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public abstract partial class AbstractShopIconContainer : Button
{

	public List<AbstractPlaceable> Products = [];
	public List<Road> RoadProducts = [];
	public PackedScene ShopIconScene;
	public BoxContainer ShopIconContainer;
	public List<Control> Stock = [];
	public Shop GameShop;
	public override void _Ready()
	{
		
		ShopIconScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/ShopIcon.tscn");
		ShopIconContainer = GetNode<BoxContainer>("ShopItemNode");
		GameShop = GetParent().GetParent<Shop>();
		AddProducts();
		foreach (var product in Products)
		{
			AddChild(product);
			var productImage = product.AnimatedSprite.SpriteFrames.GetFrameTexture("default",0);
			var shopIconControl = ShopIconScene.Instantiate<Control>();
			var shopIcon = shopIconControl.GetNode<TextureButton>("TextureButton");
			var colorRect = shopIcon.GetNode<ColorRect>("ColorRect");
			
			var containerHeight = ShopIconContainer.Size.Y;
			var scale = containerHeight / productImage.GetSize().Y;
			var size = scale * productImage.GetSize().X;
			
			shopIconControl.SetCustomMinimumSize(new Vector2(size,0));
			colorRect.Size = new Vector2(size, containerHeight);
			colorRect.Position = shopIcon.Position;
			shopIcon.TextureNormal = productImage;
			ShopIconContainer.AddChild(shopIconControl);
			RemoveChild(product);
			Stock.Add(shopIconControl);
			shopIcon.Pressed +=() => { OnShopIconPressed(product);};
			shopIcon.MouseEntered += () => {OnMouseEntered(product, shopIconControl); };
			shopIcon.MouseExited += () => {OnMouseExited(shopIconControl); };
			if (!product.isUnlocked)
			{
				shopIcon.Disabled = true;
			}
		}

		if (this is Roads)
		{
			foreach (var road in RoadProducts)
			{
				AddChild(road);
				var productImage = road.GetNode<Sprite2D>("Sprite2D").Texture;
				var shopIconControl = ShopIconScene.Instantiate<Control>();
				var shopIcon = shopIconControl.GetNode<TextureButton>("TextureButton");

				var containerHeight = ShopIconContainer.Size.Y;
				var scale = containerHeight / (productImage.GetSize().Y) ;
				var size = scale * productImage.GetSize().X;
			
				shopIcon.SetCustomMinimumSize(new Vector2(size,0));
				shopIcon.TextureNormal = productImage;
				ShopIconContainer.AddChild(shopIconControl);
				Stock.Add(shopIcon);
				RemoveChild(road);
				shopIcon.Pressed += () => { OnRoadButtonPressed(road);};
				shopIcon.MouseEntered += () => { OnMouseEntered(road);};

			}
		}
	}

	public void OnMouseEntered(Road road)
	{
		
	}
	
	public void OnMouseEntered(AbstractPlaceable product, Control shopIconControl)
	{
		var info = shopIconControl.GetNode<ProductionInfo>("ProductionInfo");
		info.setInfo(product);
		info.Visible = true;
	}

	public void OnMouseExited( Control shopIconControl)
	{
		var info = shopIconControl.GetNode<ProductionInfo>("ProductionInfo");
		info.Visible = false;
	}
	
	public void OnRoadButtonPressed(Road road)
	{
		GameShop.EmitSignal(Shop.SignalName.OnRoadBuild, road);
	}

	public void OnShopIconPressed(AbstractPlaceable item)
	{
		GameShop.EmitSignal(Shop.SignalName.OnBuildingButtonPressed, item);
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
	}
}
