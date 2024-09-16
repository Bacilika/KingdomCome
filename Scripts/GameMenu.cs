using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;

public partial class GameMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	
	private AbstractPlaceable _object;
	public static AbstractPlaceable SelectedPlaceable;
	public static bool ContainHouse;
	private int _money = 50000;
	public static int Citizens;
	public static int Happiness;
	public static int Food;
	public static int Stone;
	public static int WorkingCitizens; 
	private Godot.Collections.Dictionary<string, PackedScene> _packedScenesscenes;
	private Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	private TileMapLayer _shopBackground;
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		var currentScale = (Vector2)GetTree().Root.Size / GetTree().Root.MinSize;
		var container = GetNode<Control>("MenuCanvasLayer/Container");
		container.Scale = currentScale;
		
		_shopBackground = GetNode<TileMapLayer>("MenuCanvasLayer/Container/ShopBackground");
		
		
		var statLabels = GetNode<GridContainer>("MenuCanvasLayer/Container/GameStats");
		
		
		_packedScenesscenes = new Godot.Collections.Dictionary<string, PackedScene> { 
			{ "House", ResourceLoader.Load<PackedScene>("res://Scenes/House.tscn") },
			{"FarmHouse", ResourceLoader.Load<PackedScene>("res://Scenes/FarmHouse.tscn")},
			{"StoneMine", ResourceLoader.Load<PackedScene>("res://Scenes/StoneMine.tscn")}
		};
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label> { 
			{"money", statLabels.GetNode<Label>("Money") },
			{"food", statLabels.GetNode<Label>("Food") },
			{"citizens", statLabels.GetNode<Label>("Citizens") },
			{"stone",statLabels.GetNode<Label>("Stone")}, 
			{"happiness",statLabels.GetNode<Label>("Happiness")}
		};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();

	}
	public void OnHouseButtonPressed(String type)
	{
		Console.WriteLine(type);
		var house = _packedScenesscenes[type].Instantiate<AbstractPlaceable>();
		house.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(house);
		_object = house;
	}

	public override void _Input(InputEvent @event)
	{
		if (_object == null)
		{
			if (SelectedPlaceable is not null && @event.IsActionPressed(Inputs.LeftClick))
			{
				SelectedPlaceable.ShowBuildingInfoScreen();
			}
			
		}

		else if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if (CanPlace())
			{
				_money -= _object.GetBuildingPrice();
				var placedHouse =  _object.Duplicate();
				EmitSignal(SignalName.HousePlaced, placedHouse);
			}

		}

		if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object?.QueueFree();
			_object = null;
		}
	}

	public void UpdateMenuInfo()
	{
		_gameStatLabels["money"].Text = "Money: " + _money;
		_gameStatLabels["citizens"].Text = "Citizens: " + Citizens;
		_gameStatLabels["happiness"].Text = "Happiness: " + Happiness;
		_gameStatLabels["food"].Text = "Food: " + Food;
		_gameStatLabels["stone"].Text = "Stone: " + Stone;
	}

	private bool CanPlace()
	{
		var collisionObj = _object.GetNode<CollisionShape2D>("CollisionShape2D");
		var size = collisionObj.Shape.GetRect().Size;
		var occupiedCoords = GetOccupiedCoords(size, _object.Position);
		//var tilemap = GetNode<TileMapLayer>("res://Scenes/base/BuildingLayer");
		return ContainHouse == false && _object.GetBuildingPrice() <= _money;
	}

	private List<Vector2I> GetOccupiedCoords(Vector2 objectSize, Vector2 position)
	{
		var startX = (int) position.X / 16;
		var startY = (int)position.Y / 16;
		var tileWidth = (int) objectSize.X / 16;
		var tileHeight = (int) objectSize.Y / 16;
		var coords = new List<Vector2I>();
		for (var x = startX; x < startX + tileWidth; x++)
		{
			for (var y = startY; y < startY + tileHeight; y++)
			{
				coords.Add(new Vector2I(x,y));
			}
		}
		return coords;
	}

	

	public void OnBuildButtonPressed(string tabPath)
	{
		var currentTab = GetNode<Node2D>(tabPath);
		foreach (var node in _shopBackground.GetChildren())
		{
			var child = (Node2D)node;
			if (child != currentTab)
			{
				child.Visible = false;
			}
		}

		currentTab.Visible = !currentTab.Visible;
		_shopBackground.Visible = currentTab.Visible;


	}
}
