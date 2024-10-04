using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using Scripts.Constants;

public partial class GameLogistics: Node2D
{
	private GameMap _gameMap;
	
	private AbstractPlaceable _object;
	private Road _roadObject;
	public static bool IsPlaceMode;
	private bool _roadPlaceMode;
	private bool _containBuilding;
	public static int Day = 0;
	public static System.Collections.Generic.Dictionary<string, int> Resources; 
	public static bool dragging;
	public static bool Move;
	private PackedScene _roadScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Road.tscn");
	public static int RoadPrice = 100;
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
		shop.OnRoadBuild += OnRoadBuild;

		Resources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ GameResource.Money, 0 }, { GameResource.Citizens, 0 },
			{ GameResource.Happiness, 0 }, { GameResource.Food, 0 },
			{ GameResource.Stone, 100 }, { GameResource.Iron, 0 },
			{ GameResource.Unemployed, 0 }, { GameResource.Water, 0 },
			{ GameResource.Wood, 100 }
		};
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

		if (_roadObject is not null)
		{
			_roadObject.Position = GetGlobalMousePosition();
		}
		
	}

	public static String ConvertHappiness(int happiness)
	{
		switch (happiness)
		{
			case <= 2:
				return "Very unhappy";
			case >= 3 and <= 4:
				return "Unhappy";
			case >= 5 and <= 6:
				return "Neutral";
			case >= 7 and <= 8:
				return "Happy";
			default:
				return "Very happy";
		}
	}

	private void OnRoadBuild(Road road)
	{
		_roadPlaceMode = true;
		IsPlaceMode = true;
		_roadObject = road;
		GetParent().AddChild(_roadObject);
		GameMenu.GameMode.Text = GameMode.Road;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			dragging = true;
			if (_object != null)
			{
				if (CanPlace() && CanAfford())
				{
					if (!Move ) //Building new house
					{
						var UpgradesClass = _object.GetType().Name;
						Console.WriteLine(UpgradesClass);
						RemoveResources();
						var placedNode = _object.Duplicate();
						EmitSignal(SignalName.HousePlaced, placedNode); //Emitted to GameMap
						var placedBuilding = (AbstractPlaceable)placedNode;
						placedBuilding.OnMoveBuilding += OnMove;
						placedBuilding.OnAreaUpdated += SetContainsBuilding;
						placedBuilding.OnBuildingUpgrade += UpgradeBuilding;
						Shop.placeAudio.Play();
						_containBuilding = true;
						ResetModes();
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
			ResetModes();
		}
		if (@event.IsActionReleased(Inputs.LeftClick))
		{
			dragging = false;
		}
	}

	public static bool HasUnemployedCitizens()
	{
		return Resources[GameResource.Unemployed] > 0;
	}
	

	public bool CanPlace()
	{
		return _containBuilding == false;
	}

	public void ResetModes()
	{
		if(_roadObject != null) GetParent().RemoveChild(_roadObject);
		if(_object != null && !Move) GetParent().RemoveChild(_object);
		_object = null;
		_roadObject = null;
		IsPlaceMode = false;
		_roadPlaceMode = false;
		Move = false;
		GameMenu.GameMode.Text = "";
		
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
			foreach (var cost in _building.BuildCost)
			{
				if (cost.Value[_building.Level] >= Resources[cost.Key])
				{
					return false;
				}
			}

			return true;

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
		GameMenu.GameMode.Text = GameMode.Move;
	}

	public void SetContainsBuilding(bool status)
	{
		_containBuilding = status;
	}
	private void UpgradeBuilding(AbstractPlaceable building)
	{
		if(CanAfford(building))
		{
			RemoveResources(building);
			building.SetObjectValues();
		}
		else
		{
			building.Level--;
		}
	}

	public void RemoveResources(AbstractPlaceable building = null)
	{
		if (building is null)
		{
			building = _object;
		}
		foreach (var cost in building.BuildCost)
		{
			Resources[cost.Key] -= cost.Value[building.Level];
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
		GameMenu.GameMode.Text = GameMode.Build;
		
	}
	
	private void PlaceRoad()
	{
		if (CanPlaceRoad())
		{
			var gridPosition = _roadLayer.LocalToMap( GetGlobalMousePosition());
			_roadPositions.Add(gridPosition);
			_roadLayer.SetCellsTerrainConnect( _roadPositions, 0, 0);
			Resources[GameResource.Money] -= RoadPrice;

		}
	}
	
	private bool CanPlaceRoad()
	{
		return !_roadPositions.Contains(_roadLayer.LocalToMap(GetGlobalMousePosition()));
	}
	
}
