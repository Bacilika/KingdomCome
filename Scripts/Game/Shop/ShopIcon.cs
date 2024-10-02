using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopIcon : Control
{
	public TextureButton Icon;
	public ColorRect Color;
	public Shop Shop;
	private ProductionInfo _productInfo;

	public AbstractPlaceable Product;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_productInfo = GetNode<ProductionInfo>("ProductionInfo");
		Icon = GetNode<TextureButton>("TextureButton");
		Color = Icon.GetNode<ColorRect>("ColorRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public AbstractPlaceable GetProduct()
	{
		return Product;
	}

	public void SetUp(AbstractPlaceable product, Shop shop)
	{
		Product = product;
		Shop = shop;
		Texture2D productImage;
		productImage = product.AnimatedSprite.SpriteFrames.GetFrameTexture("default",0);
		
		
		var containerHeight = Size.Y;
		var size = containerHeight / productImage.GetSize().Y * productImage.GetSize().X;
			
		SetCustomMinimumSize(new Vector2(size,0));
		Color.Size = new Vector2(size, containerHeight);
		Color.Position = Icon.Position;
		Icon.TextureNormal = productImage;
		
		
		Icon.Pressed += OnShopIconPressed;
		Icon.MouseEntered += OnMouseEntered;
		Icon.MouseExited += OnMouseExited;
		if (!product.isUnlocked)
		{
			Icon.Disabled = true;
		}

		
	}
	public void OnMouseEntered()
	{
		_productInfo.setInfo(Product);
		_productInfo.Visible = true;
	}

	public void OnMouseExited()
	{
		_productInfo.Visible = false;
	}
	
	public void OnShopIconPressed()
	{
		if (Product is Road road)
		{
			Shop.EmitSignal(Shop.SignalName.OnRoadBuild, road);
			
		}
		else //road
		{
			Shop.EmitSignal(Shop.SignalName.OnBuildingButtonPressed, Product);
			
		}
		
	}
}
