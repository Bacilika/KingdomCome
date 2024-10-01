#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Scripts.Constants;

public partial class Shop : Control
{
	private Godot.Collections.Dictionary<string, PackedScene> _shopItems;
	// Called when the node enters the scene tree for the first time.
	public static AudioStreamPlayer2D placeAudio;
	public static AudioStreamPlayer2D deleteAudio;
	private int _roadPrice = 100;

	private List<AbstractShopIconContainer> _shopTabs;
	//Make overall level. If level > x, unlock certain buildings. 
	private bool _locked = true;
	private Control _productionInfo;


	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);

	[Signal]
	public delegate void OnRoadBuildEventHandler(Road road);
	public override void _Ready()
	{
		_shopItems = new Godot.Collections.Dictionary<string, PackedScene> { 
			{ "House", ResourceLoader.Load<PackedScene>("res://Scenes/Building/LivingSpace/House.tscn") },
			{"FarmHouse", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/FarmHouse.tscn")},
			{"StoneMine", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/StoneMine.tscn")},
			{"HunterLodge", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/HunterLodge.tscn")},
			{"WoodCutter", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/WoodCutter.tscn")},
			{"IronMine", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/IronMine.tscn")},
			{"Well", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/Well.tscn")},
			{"MarketStall", ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/MarketStall.tscn")}
		};

		placeAudio = GetNode<AudioStreamPlayer2D>("PlaceBuildingAudio");
		deleteAudio = GetNode<AudioStreamPlayer2D>("DeleteBuildingAudio");
		
		_shopTabs = [GetNode<AbstractShopIconContainer>("BuildTabButtons/Houses"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Productions"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Decorations"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Roads")];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnBuildTabPressed(string tabPath)
	{
		foreach (var tab in _shopTabs)
		{
			if (tab.Name == tabPath)
			{
				tab.ShowStock();
			}
			else
			{
				tab.HideStock();
			}
		}
	}
	public void OnHuntingButtonMouseEntered(string buttonPath)
	{
		var button = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/" + buttonPath);
		_productionInfo = button.GetNode<Control>("ProductionInfo");
		_productionInfo.Visible = true;
	}

	public void OnHuntingButtonMouseExited()
	{
		_productionInfo.Visible = false;
	}
}
