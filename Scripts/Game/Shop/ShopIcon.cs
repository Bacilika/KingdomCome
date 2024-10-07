using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopIcon : Control
{
	public TextureButton Icon;
	public Shop Shop;
	private ProductionInfo _productionInfo;

	public AbstractPlaceable Product;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Icon = GetNode<TextureButton>("TextureButton");
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
		Icon.TextureNormal = product.AnimatedSprite.SpriteFrames.GetFrameTexture("default",0);
		var containerHeight = Size.Y;
		var minimum = new Vector2(containerHeight, containerHeight);
		SetCustomMinimumSize(minimum);
		Icon.SetCustomMinimumSize(minimum);
		CustomMinimumSize = minimum;
		
		Icon.Pressed += OnShopIconPressed;
		Icon.MouseEntered += OnMouseEntered;
		Icon.MouseExited += OnMouseExited;
		if (!product.isUnlocked)
		{
			Icon.Disabled = false;
		}

		
	}
	public void OnMouseEntered()
	{
		if (_productionInfo is null)
		{
			_productionInfo = Shop.GetParent().GetParent().GetParent<GameMenu>().ProductionInfo;
		}
		_productionInfo.setInfo(Product);
		_productionInfo.Visible = true;
	}

	public void OnMouseExited()
	{
		_productionInfo.Visible = false;
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
