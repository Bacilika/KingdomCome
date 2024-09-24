using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;

public partial class GameMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	
	private AbstractPlaceable _object;
	private Road _roadObject;
	public static bool IsPlaceMode;
	public static bool RoadPlaceMode;
	public static bool ContainHouse;
	public static int Money = 50000;
	public static int Citizens;
	public static int Happiness;
	public static int Food;
	public static int Stone = 2000;
	public static int Wood = 2000;
	public static int WorkingCitizens;
	public static int Day = 0;
	public static bool dragging;
	public static bool Move;
	private PackedScene _roadScene = ResourceLoader.Load<PackedScene>("res://Scenes/Road.tscn");
	private int _roadPrice = 100;
	private Array<Vector2I> _roadPositions = [];
	private TileMapLayer _roadLayer;
	private static Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		var currentScale = (Vector2)GetTree().Root.Size / GetTree().Root.MinSize;
		var container = GetNode<Control>("MenuCanvasLayer/Container");
		container.Scale = currentScale;
		var statLabels = GetNode<GridContainer>("MenuCanvasLayer/Container/GameStats");
		_gameStatLabels = new Godot.Collections.Dictionary<string, Label> { 
			{"money", statLabels.GetNode<Label>("Money") },
			{"food", statLabels.GetNode<Label>("Food") },
			{"citizens", statLabels.GetNode<Label>("Citizens") },
			{"stone",statLabels.GetNode<Label>("Stone")}, 
			{"happiness",statLabels.GetNode<Label>("Happiness")},
			{"day",statLabels.GetNode<Label>("Day")},
			{"wood",statLabels.GetNode<Label>("Wood")}
		};
		_roadLayer = GetNode<TileMapLayer>("../RoadLayer");
		var shop = GetNode<Shop>("MenuCanvasLayer/Container/Shop");
		shop.OnBuildingButtonPressed += BuildBuilding;
		shop.Connect(Shop.SignalName.OnRoadBuild,Callable.From(OnRoadBuild));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();
		if (dragging && RoadPlaceMode)
		{
			PlaceRoad();
		}
		if (_object is not null)
		{
			_object.Position = GetGlobalMousePosition();
		}

		if (_object is null && Move)
		{
			
		}
	}

	private void OnRoadBuild()
	{
		RoadPlaceMode = true;
		IsPlaceMode = true;
		_roadObject = _roadScene.Instantiate<Road>();
		GetParent().AddChild(_roadObject);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			dragging = true;
			if (_object != null)
			{
				GD.Print("Can afford " + CanAfford() );
				GD.Print("Can place " + CanPlace() );
				if (CanPlace() && CanAfford())
				{
					if (!Move )
					{
							Money -= _object.GetPrice();
							Wood -= _object.Upgrades["WoodCost"][_object.Level];
							Stone -= _object.Upgrades["StoneCost"][_object.Level];
							var placedHouse = _object.Duplicate();
							EmitSignal(SignalName.HousePlaced, placedHouse); //Emitted to GameMap
							((AbstractPlaceable)placedHouse).OnMoveBuilding += OnMove;
							Shop.placeAudio.Play();
							ContainHouse = true;

					}
					else
					{
						_object.IsPlaced = true;
						Move = false;
						_object = null;
						IsPlaceMode = false;
					}
					
					
				}
			}
		}
		if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object?.QueueFree();
			_roadObject?.QueueFree();
			_object = null;
			_roadObject = null;
			IsPlaceMode = false;
			RoadPlaceMode = false;
			Move = false;
		}
		if (@event.IsActionReleased(Inputs.LeftClick))
		{
			dragging = false;
		}
	}

	public static void UpdateMenuInfo()
	{
		_gameStatLabels["money"].Text = "Money: " + Money;
		_gameStatLabels["citizens"].Text = "Citizens: " + Citizens;
		_gameStatLabels["happiness"].Text = "Happiness: " + Happiness;
		_gameStatLabels["food"].Text = "Food: " + Food;
		_gameStatLabels["stone"].Text = "Stone: " + Stone;
		_gameStatLabels["day"].Text = "Day: " + Day;
		_gameStatLabels["wood"].Text = "Wood: " + Wood;
	}

	public bool CanPlace()
	{
		return ContainHouse == false;
	}

	public bool CanAfford()
	{
		if (!Move)
		{
			return _object.Upgrades["WoodCost"][_object.Level] <= Wood && 
				   _object.Upgrades["StoneCost"][_object.Level] <= Stone && 
				   _object.GetPrice() <= Money;
		}
		// add cost for moving house here
		else
		{
			return true;
		}
	}

	public void OnMove(AbstractPlaceable building)
	{
		_object = building;
		ContainHouse = false;
	}

	private void BuildBuilding(AbstractPlaceable building)
	{
		building.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(building);
		_object = building;
		IsPlaceMode = true;
		
	}
	
	private void PlaceRoad()
	{
		if (CanPlaceRoad())
		{
			var gridPosition = _roadLayer.LocalToMap( GetGlobalMousePosition());
			_roadPositions.Add(gridPosition);
			_roadLayer.SetCellsTerrainConnect( _roadPositions, 0, 0);
			Money -= _roadPrice;

		}
	}
	
	private bool CanPlaceRoad()
	{
		return !_roadPositions.Contains(_roadLayer.LocalToMap(GetGlobalMousePosition()));
	}
}
