using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using Scripts.Constants;

public partial class GameLogistics: Node2D
{
	private GameMap _gameMap;
	private List<House> _houses;
	private List<Production> _productions;
	
	private AbstractPlaceable _object;
	private Road _roadObject;
	public static bool IsPlaceMode;
	private bool _roadPlaceMode;
	private bool _containBuilding;
	public static int _money = 50000;
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
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		_gameMap = GetParent<GameMap>();
		_roadLayer = _gameMap.GetNode<TileMapLayer>("RoadLayer");
		var gameMenu = _gameMap.GetNode<Control>("GameMenu");
		var shop = gameMenu.GetNode<Shop>("MenuCanvasLayer/Container/Shop");
		shop.OnBuildingButtonPressed += BuildBuilding;
		shop.Connect(Shop.SignalName.OnRoadBuild,Callable.From(OnRoadBuild));
		
		_houses = _gameMap._placedHouses;
		_productions = _gameMap._placedProduction;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (dragging && _roadPlaceMode)
		{
			PlaceRoad();
		}
		if (_object is not null)
		{
			_object.Position = GetGlobalMousePosition();
		}
		
	}

	private void OnRoadBuild()
	{
		_roadPlaceMode = true;
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
							_money -= _object.GetPrice();
							Wood -= _object.Upgrades["WoodCost"][_object.Level];
							Stone -= _object.Upgrades["StoneCost"][_object.Level];
							var placedNode = _object.Duplicate();
							EmitSignal(SignalName.HousePlaced, placedNode); //Emitted to GameMap
							var placedBuilding = (AbstractPlaceable)placedNode;
							placedBuilding.OnMoveBuilding += OnMove;
							placedBuilding.OnAreaUpdated += SetContainsBuilding;
							placedBuilding.OnBuildingUpgrade += UpgradeBuilding;
							
							Shop.placeAudio.Play();
							_containBuilding = true;
					}
					else
					{
						_object.IsPlaced = true;
						ResetModes();
					}
					
				}
			}
		}
		if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object?.QueueFree();
			_roadObject?.QueueFree();
			ResetModes();
		}
		if (@event.IsActionReleased(Inputs.LeftClick))
		{
			dragging = false;
		}
	}
	

	public bool CanPlace()
	{
		return _containBuilding == false;
	}

	public void ResetModes()
	{
		_object = null;
		_roadObject = null;
		IsPlaceMode = false;
		_roadPlaceMode = false;
		Move = false;
	}

	public bool CanAfford(AbstractPlaceable building = null)
	{
		var _building = building;
		if (building is null)
		{
			_building = _object;
		}
		if (!Move)
		{
			return _building.Upgrades["WoodCost"][_building.Level] <= Wood &&
				   _building.Upgrades["StoneCost"][_building.Level] <= Stone;
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
		_containBuilding = false;
		Move = true;
		IsPlaceMode = true;
	}

	public void SetContainsBuilding(bool status)
	{
		_containBuilding = status;
	}
	private void UpgradeBuilding(AbstractPlaceable building)
	{
		if(CanAfford(building))
		{
			Stone -= building.Upgrades["StoneCost"][building.Level];
			Wood -= building.Upgrades["WoodCost"][building.Level];
			building.SetObjectValues();
		}
		else
		{
			building.Level--;
		}
	}

	private void BuildBuilding(AbstractPlaceable building)
	{
		building.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(building);
		_object = building;
		_containBuilding = false;
		IsPlaceMode = true;
		
	}
	
	private void PlaceRoad()
	{
		if (CanPlaceRoad())
		{
			var gridPosition = _roadLayer.LocalToMap( GetGlobalMousePosition());
			_roadPositions.Add(gridPosition);
			_roadLayer.SetCellsTerrainConnect( _roadPositions, 0, 0);
			_money -= _roadPrice;

		}
	}
	
	private bool CanPlaceRoad()
	{
		return !_roadPositions.Contains(_roadLayer.LocalToMap(GetGlobalMousePosition()));
	}
	
}
