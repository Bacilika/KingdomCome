using Godot;
using Godot.Collections;
using KingdomCome.Scripts.Building;
using Scripts.Constants;

public partial class GameLogistics : Node2D
{
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);

	public static bool IsPlaceMode;
	public static int Day = 0;
	public static System.Collections.Generic.Dictionary<string, int> Resources;
	public static System.Collections.Generic.Dictionary<string, int> FoodResource;
	public static System.Collections.Generic.Dictionary<string, int> ProcessedResources;
	public static bool dragging;
	public static bool Move;
	private bool _containBuilding;
	private GameMap _gameMap;

	private AbstractPlaceable _object;
	private TileMapLayer _roadLayer;
	private Road _roadObject;
	private bool _roadPlaceMode;
	private Array<Vector2I> _roadPositions = [];
	private PackedScene _roadScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Road.tscn");


	public override void _Ready()
	{
		_gameMap = GetParent<GameMap>();
		_roadLayer = _gameMap.GetNode<TileMapLayer>("RoadLayer");
		var gameMenu = _gameMap.GetNode<Control>("GameMenu");
		var shop = gameMenu.GetNode<Shop>("MenuCanvasLayer/Container/Shop");
		shop.OnBuildingButtonPressed += BuildBuilding;
		shop.OnRoadBuild += OnRoadBuild;

		//_houses = _gameMap._placedHouses;
		//_productions = _gameMap._placedProduction;

		Resources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ RawResource.Money, 0 }, { RawResource.Citizens, 0 },
			{ RawResource.Happiness, 0 }, { RawResource.Food, 0 },
			{ RawResource.Stone, 100 }, { RawResource.Iron, 0 },
			{ RawResource.Unemployed, 0 }, { RawResource.Water, 0 },
			{ RawResource.Wood, 100 }
		};
		
		FoodResource = new System.Collections.Generic.Dictionary<string, int>
		{
			{ Food.Bread, 0 }, { Food.Meat, 0 }, { Food.Crops, 0 }
		};
		
		ProcessedResources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ ProcessedResource.Plank, 0 }, { ProcessedResource.IronIngot, 0 }
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (dragging && _roadPlaceMode) PlaceRoad();
		if (_object is not null) _object.Position = GetGlobalMousePosition();

		if (_roadObject is not null) _roadObject.Position = GetGlobalMousePosition();
	}

	public static string ConvertHappiness(int happiness)
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
				GD.Print("Can afford " + CanAfford());
				GD.Print("Can place " + CanPlace());
				if (CanPlace() && CanAfford())
				{
					if (!Move)
					{
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

		if (@event.IsActionPressed(Inputs.RightClick)) ResetModes();
		if (@event.IsActionReleased(Inputs.LeftClick)) dragging = false;
	}

	public static bool HasUnemployedCitizens()
	{
		return Resources[RawResource.Unemployed] > 0;
	}


	public bool CanPlace()
	{
		return _containBuilding == false;
	}

	public void ResetModes()
	{
		if (_roadObject != null) GetParent().RemoveChild(_roadObject);
		if (_object != null && !Move) GetParent().RemoveChild(_object);
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
		if (building is null) _building = _object;
		if (!Move)
		{
			foreach (var cost in _building.BuildCost)
				if (cost.Value[_building.Level] > Resources[cost.Key])
					return false;

			return true;
		}
		// add cost for moving house here

		return true;
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
		if (CanAfford(building))
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
		if (building is null) building = _object;
		foreach (var cost in building.BuildCost) Resources[cost.Key] -= cost.Value[building.Level];
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
			var gridPosition = _roadLayer.LocalToMap(GetGlobalMousePosition());
			_roadPositions.Add(gridPosition);
			_roadLayer.SetCellsTerrainConnect(_roadPositions, 0, 0);
			RemoveResources(_roadObject);
		}
	}

	private bool CanPlaceRoad()
	{
		return !_roadPositions.Contains(_roadLayer.LocalToMap(GetGlobalMousePosition())) && CanAfford(_roadObject);
	}
}
