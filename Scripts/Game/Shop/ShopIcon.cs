using Godot;
using KingdomCome.Scripts.Building;

public partial class ShopIcon : Control
{
	private ProductionInfo _productionInfo;
	public TextureButton Icon;
	public Sprite2D RoadRemoval;

	public AbstractPlaceable Product;

	public Shop Shop;

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
		Icon.TextureNormal = product.HouseSprite.SpriteFrames.GetFrameTexture("Level0", 0);
		var containerHeight = Size.Y;
		var minimum = new Vector2(containerHeight, containerHeight);
		SetCustomMinimumSize(minimum);
		Icon.SetCustomMinimumSize(minimum);
		CustomMinimumSize = minimum;

		Icon.Pressed += OnShopIconPressed;
		Icon.MouseEntered += OnMouseEntered;
		Icon.MouseExited += OnMouseExited;
		if (!product.IsUnlocked) Icon.Disabled = false;
	}

	public void AddRoadRemoval()
	{
		RoadRemoval = new Sprite2D();
		var texture = (Texture2D)GD.Load("res://Sprites/The Fan-tasy Tileset (Premium)/Art/Props/Shovel.png/");
		Icon.TextureNormal = texture;
		RoadRemoval.Texture = texture;
		var containerHeight = Size.Y;
		var minimum = new Vector2(containerHeight, containerHeight);
		SetCustomMinimumSize(minimum);
		Icon.SetCustomMinimumSize(minimum);
		CustomMinimumSize = minimum;
		Icon.Pressed += OnShopIconPressed;
		Icon.MouseEntered += OnMouseEntered;
		Icon.MouseExited += OnMouseExited;

	}

	public void OnMouseEntered()
	{
		if (RoadRemoval is not null)
		{
			TooltipText = "Remove a Road tile. You get back full road cost";
			return;
		}
		if (_productionInfo is null)
			_productionInfo = Shop.GetParent().GetParent<GameMenu>().ProductionInfo;
		_productionInfo.setInfo(Product);
		_productionInfo.Visible = true;
	}

	public void OnMouseExited()
	{
		if (RoadRemoval is not null)
		{
			return;
		}
		_productionInfo.Visible = false;
	}

	public void OnShopIconPressed()
	{
		if (RoadRemoval is not null)
		{
			Shop.EmitSignal(Shop.SignalName.OnRoadRemove, RoadRemoval);
			return;
		}
		if (Product is Road road)
			Shop.EmitSignal(Shop.SignalName.OnRoadBuild, road);
		else // not road
			Shop.EmitSignal(Shop.SignalName.OnBuildingButtonPressed, Product);
	}
}
