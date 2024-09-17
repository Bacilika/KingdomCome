#nullable enable
using Godot;
using System;
using System.Diagnostics;
using Scripts.Constants;

public partial class Shop : Control
{
	private Godot.Collections.Dictionary<string, PackedScene> _shopItems;
	// Called when the node enters the scene tree for the first time.
	private GridContainer _buildButtons;

	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);
	public override void _Ready()
	{
		_shopItems = new Godot.Collections.Dictionary<string, PackedScene> { 
			{ "House", ResourceLoader.Load<PackedScene>("res://Scenes/House.tscn") },
			{"FarmHouse", ResourceLoader.Load<PackedScene>("res://Scenes/FarmHouse.tscn")},
			{"StoneMine", ResourceLoader.Load<PackedScene>("res://Scenes/StoneMine.tscn")}
		};
		_buildButtons = GetNode<GridContainer>("BuildTabButtons");
		var house = GetNode<Button>("BuildTabButtons/Houses/ShopItemNode/HouseButton");
		house.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("House"); }));
		var farmHouse = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/FarmButton");
		farmHouse.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("FarmHouse"); }));
		var stoneMine = GetNode<Button>("BuildTabButtons/Production/ShopItemNode/StoneButton");
		stoneMine.Connect(Signals.Pressed, Callable.From(() => {OnBuildButtonPressed("StoneMine"); }));
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
	public void OnBuildButtonPressed(string type)
	{
		Console.WriteLine(type);
		var house = _shopItems[type].Instantiate<AbstractPlaceable>();
		EmitSignal(SignalName.OnBuildingButtonPressed, house);

		
	}
}
