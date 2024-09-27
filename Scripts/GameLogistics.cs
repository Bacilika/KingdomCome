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
	public static int Day = 0;
	public static System.Collections.Generic.Dictionary<string, int> Resources; 
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

		Resources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ "Money", 100000 }, { "Citizens", 0 }, { "Happiness", 0 }, { "Food", 0 }, { "Stone", 100 }, { "Iron", 0 },
			{ "WorkingCitizens", 0 },
			{ "Water", 0 }, { "Wood",100 }
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
							Resources["Money"] -= _object.GetPrice();
							Resources["Wood"] -= _object.Upgrades[Upgrade.WoodCost][_object.Level];
							Resources["Stone"] -= _object.Upgrades[Upgrade.StoneCost][_object.Level];
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

	public static bool HasUnemployedCitizens()
	{
		return Resources["WorkingCitizens"] < Resources["Citizens"];
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
			return _building.Upgrades[Upgrade.WoodCost][_building.Level] <= Resources["Wood"] &&
				   _building.Upgrades[Upgrade.StoneCost][_building.Level] <= Resources["Stone"];
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
			Resources["Stone"] -= building.Upgrades[Upgrade.StoneCost][building.Level];
			Resources["Wood"] -= building.Upgrades[Upgrade.WoodCost][building.Level];
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
			Resources["Money"] -= _roadPrice;

		}
	}
	
	private bool CanPlaceRoad()
	{
		return !_roadPositions.Contains(_roadLayer.LocalToMap(GetGlobalMousePosition()));
	}
	
}
