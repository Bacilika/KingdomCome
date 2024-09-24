#nullable enable
using Godot;
using System;
using System.Diagnostics;
using KingdomCome.Scripts;

public partial class Shop : Control
{
	private Godot.Collections.Dictionary<string, PackedScene> _shopItems;
	// Called when the node enters the scene tree for the first time.
	private GridContainer _buildButtons;
	public static AudioStreamPlayer2D placeAudio;
	private int _roadPrice = 100;


	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);

	[Signal]
	public delegate void OnRoadBuildEventHandler();
	public override void _Ready()
	{
		_shopItems = new Godot.Collections.Dictionary<string, PackedScene> { 
			{ "House", ResourceLoader.Load<PackedScene>("res://Scenes/House.tscn") },
			{"FarmHouse", ResourceLoader.Load<PackedScene>("res://Scenes/FarmHouse.tscn")},
			{"StoneMine", ResourceLoader.Load<PackedScene>("res://Scenes/StoneMine.tscn")},
			{"HunterLodge", ResourceLoader.Load<PackedScene>("res://Scenes/HunterLodge.tscn")},
			{"WoodCutter", ResourceLoader.Load<PackedScene>("res://Scenes/WoodCutter.tscn")}
		};
		_buildButtons = GetNode<GridContainer>("BuildTabButtons");
		var house = GetNode<Button>("BuildTabButtons/Houses/ShopItemNode/HouseButton");
		house.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("House", "BuildTabButtons/Houses/ShopItemNode/HouseButton" ); }));
		var farmHouse = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/FarmButton");
		farmHouse.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("FarmHouse", "BuildTabButtons/Production/ShopItemNode/FarmButton"); }));
		var stoneMine = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/StoneButton");
		stoneMine.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("StoneMine","BuildTabButtons/Production/ShopItemNode/StoneButton"); }));
		var road = GetNode<Button>("BuildTabButtons/Roads/ShopItemNode/RoadButton");
		road.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("Road","BuildTabButtons/Roads/ShopItemNode/RoadButton"); }));
		var huntersLodge = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/HuntingButton");
		huntersLodge.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("HunterLodge","BuildTabButtons/Production/ShopItemNode/HuntingButton"); }));
		var woodCutter = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/WoodButton");
		woodCutter.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("WoodCutter","BuildTabButtons/Production/ShopItemNode/WoodButton"); }));

		placeAudio = GetNode<AudioStreamPlayer2D>("PlaceBuildingAudio");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnBuildTabPressed(string tabPath)
	{
		var pressedTabButton = _buildButtons.GetNode<Button>(tabPath);
		foreach (var node in _buildButtons.GetChildren())
		{
			var tabButton = (Button)node;
			var shopItemNode = tabButton.GetNode<Control>("ShopItemNode");
			if (tabButton == pressedTabButton)
			{
				if (shopItemNode != null)
				{
					shopItemNode.Visible = !shopItemNode.Visible;
				}
			}
			else if (shopItemNode != null) shopItemNode.Visible = false;
		}
	}
	
	public void OnBuildButtonPressed(string type, string buttonPath)
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
}
