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
			var shopIcon = ShopIconScene.Instantiate<TextureButton>();
			var text = shopIcon.GetNode<Label>("Text");
			var containerHeight = ShopIconContainer.Size.Y;
			var scale = containerHeight / productImage.GetSize().Y;
			var size = scale * productImage.GetSize().X;
			
			shopIcon.SetCustomMinimumSize(new Vector2(size,0));
			shopIcon.TextureNormal = productImage;
			text.Text = "Wood: " + product.Upgrades[Upgrade.WoodCost][0] + 
			            "\nStone: " + product.Upgrades[Upgrade.StoneCost][0];
			text.Position = new Vector2(shopIcon.Position.X, text.Position.Y);
			text.Size = new Vector2(shopIcon.Size.X, text.Size.Y);
			ShopIconContainer.AddChild(shopIcon);
			RemoveChild(product);
			Stock.Add(shopIcon);
			shopIcon.Pressed +=() => { OnShopIconPressed(product);};
		}

		if (this is Roads)
		{
			foreach (var road in RoadProducts)
			{
				AddChild(road);
				var productImage = road.GetNode<Sprite2D>("Sprite2D").Texture;
				var shopIcon = ShopIconScene.Instantiate<TextureButton>();
				var text = shopIcon.GetNode<Label>("Text");
				var containerHeight = ShopIconContainer.Size.Y;
				var scale = containerHeight / productImage.GetSize().Y;
				var size = scale * productImage.GetSize().X;
			                                              
				shopIcon.SetCustomMinimumSize(new Vector2(size,0));
				shopIcon.TextureNormal = productImage;
				text.Text = "Price: " + 50;
				text.Position = new Vector2(shopIcon.Position.X, text.Position.Y);
				text.Size = new Vector2(shopIcon.Size.X, text.Size.Y);
				ShopIconContainer.AddChild(shopIcon);
				Stock.Add(shopIcon);
				RemoveChild(road);
				shopIcon.Pressed += () => { OnRoadButtonPressed(road);};


			}
		}
	}

	public void OnRoadButtonPressed(Road road)
	{
		GameShop.EmitSignal(Shop.SignalName.OnRoadBuild);
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
