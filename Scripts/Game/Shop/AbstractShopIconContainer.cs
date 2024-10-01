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
			shopIcon.MouseEntered += () => {OnMouseEntered(product, shopIcon); };
			shopIcon.MouseExited += () => {OnMouseExited(shopIcon); };
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
				var scale = containerHeight / (productImage.GetSize().Y) ;
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
				shopIcon.MouseEntered += () => { OnMouseEntered(road);};

			}
		}
	}

	public void OnMouseEntered(Road road)
	{
		
	}
	
	public void OnMouseEntered(AbstractPlaceable product, TextureButton shopIcon)
	{
		var info = shopIcon.GetNode<ProductionInfo>("ProductionInfo");
		info.setInfo(product);
		info.Visible = true;
	}

	public void OnMouseExited(TextureButton shopIcon)
	{
		var info = shopIcon.GetNode<ProductionInfo>("ProductionInfo");
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
