using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public abstract partial class AbstractShopIconContainer : Button
{

	public List<AbstractPlaceable> Products = [];
	public PackedScene ShopIconScene;
	public BoxContainer ShopIconContainer;
	public List<Control> Stock = [];
	public override void _Ready()
	{
		ShopIconScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/ShopIcon.tscn");
		ShopIconContainer = GetNode<BoxContainer>("ShopItemNode");
		AddProducts();
		foreach (var product in Products)
		{
			AddChild(product);
			var productImage = product.AnimatedSprite.SpriteFrames.GetFrameTexture("default",0);
			var shopIcon = ShopIconScene.Instantiate<Control>();
			var button = shopIcon.GetNode<TextureButton>("Icon");
			var text = shopIcon.GetNode<RichTextLabel>("Text");
			
			button.TextureNormal = productImage;
			text.Text = "Wood: " + product.Upgrades[Upgrade.WoodCost][0] + 
			            "\n Stone: " + product.Upgrades[Upgrade.StoneCost][0];
			
			ShopIconContainer.AddChild(shopIcon);
			Stock.Add(shopIcon);
			RemoveChild(product);
			
		}
	}

	public abstract void AddProducts();
	
	public override void _Process(double delta)
	{
	}

	public void HideStock()
	{
		foreach (var item in Stock)
		{
			item.Visible = false;
		}
	}
	public void ShowStock()
	{
		foreach (var item in Stock)
		{
			item.Visible = true;
		}
	}
	
	/*public void OnBuildButtonPressed(string type, string buttonPath)
	{
		var button = GetNode<Button>(buttonPath);
		button.ReleaseFocus();

		if(type == "Road")
		{
			EmitSignal(SignalName.OnRoadBuild);
			return;
		}
		var house = _shopItems[type].Instantiate<AbstractPlaceable>();
		EmitSignal(SignalName.OnBuildingButtonPressed, house);
		
		
	}
	*/
}
