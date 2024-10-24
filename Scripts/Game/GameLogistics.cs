using System;
using System.IO;
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
	private Array<Vector2I> _roadPositions = [];
	
	public static bool dragging;
	public static bool Move;
	private bool _containBuilding;
	
	private GameMap _gameMap;
	private GameMenu _gameMenu;
	private AbstractPlaceable _object;
	private TileMapLayer _roadLayer;
	private Road _roadObject;
	private bool _roadPlaceMode;
	
	private PackedScene _roadScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Road.tscn");
	public static string FoodResourceAsString;
	public Sprite2D RoadRemovalTexture;
	
	
	public override void _Ready()
	{
		_gameMap = GetParent<GameMap>();
		_roadLayer = _gameMap.GetNode<TileMapLayer>("RoadLayer");
		_gameMenu = _gameMap.GetNode<GameMenu>("GameMenu");
		var shop = _gameMenu.GetNode<Shop>("MenuCanvasLayer/Shop");
		shop.OnBuildingButtonPressed += BuildBuilding;
		shop.OnRoadBuild += OnRoadBuild;
		shop.OnRoadRemove += OnRoadRemove;
		
		
		Resources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ RawResource.Money, 0 }, { RawResource.Happiness, 0 },
			{ RawResource.Food, 0 }, { RawResource.Stone, 100 },
			{ RawResource.Iron, 0 }, { RawResource.Water, 0 },
			{ RawResource.Wood, 100 }, { RawResource.Wheat, 10}
		};
		
		FoodResource = new System.Collections.Generic.Dictionary<string, int>
		{
			{ Food.Bread, 0 }, { Food.Meat, 0 }, { Food.Crops, 0 }
		};
		
		ProcessedResources = new System.Collections.Generic.Dictionary<string, int>
		{
			{ ProcessedResource.Plank, 0 }, { ProcessedResource.IronIngot, 0 },
			{ProcessedResource.Flour,0}
		};
	}
	

	private static void CalculateFoods()
	{
		FoodResourceAsString = "";

		var totalfood = 0;
		foreach (var food in FoodResource)
		{
			FoodResourceAsString += $"{food.Key}: {food.Value}\n";
			totalfood += food.Value;
		}

		Resources[RawResource.Food] = totalfood;
	}

	public static void ConsumeFood()
	{
		foreach (var food in FoodResource)
		{
			FoodResource[food.Key] --;
			Resources[RawResource.Food]--;
			return;
		}
	}
		

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_object is not null || _roadObject is not null)
		{
			_gameMenu.HideShop(true);
		}
		else
		{
			_gameMenu.HideShop(false);
		}
		CalculateFoods();
		if (dragging && _roadPlaceMode && !_gameMenu.CancelButtonFocused)
		{
			PlaceRoad();
		}
		if (_object is not null) _object.Position = _roadLayer.LocalToMap(GetGlobalMousePosition()) * 32;

		

		if (_roadObject is not null) _roadObject.Position =  _roadLayer.LocalToMap(GetGlobalMousePosition()) * 32;
		if(RoadRemovalTexture is not null) RoadRemovalTexture.Position = _roadLayer.LocalToMap(GetGlobalMousePosition()) * 32;
	}

	public static string ConvertHappiness(int happiness)
	{
		switch (happiness)
		{
			case <= 2:
				return "Very unhappy";
			case  <= 4:
				return "Unhappy";
			case  <= 6:
				return "Neutral";
			case  <= 8:
				return "Happy";
			default:
				return "Very happy";
		}
	}

	private void OnRoadBuild(Road road)
	{
		if (_object is not null)
		{
			GetParent().RemoveChild(_object);
		}
		_roadPlaceMode = true;
		IsPlaceMode = true;
		_roadObject = road;
		GetParent().AddChild(_roadObject);
		GameMenu.GameMode.Text = GameMode.Road;
	}
	private void OnRoadRemove(Sprite2D sprite2D)
	{
		_roadPlaceMode = true;
		IsPlaceMode = true;
		_roadObject = null;
		RoadRemovalTexture = sprite2D;
		GetParent().AddChild(sprite2D);
		
		GameMenu.GameMode.Text = GameMode.RoadRemove;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if(_gameMenu.CancelButtonFocused) return;
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

	


	public bool CanPlace()
	{
		return _containBuilding == false;
	}

	public void ResetModes()
	{
		if (_roadObject != null) GetParent().RemoveChild(_roadObject);
		if (RoadRemovalTexture != null) GetParent().RemoveChild(RoadRemovalTexture);
		if (_object != null && !Move) GetParent().RemoveChild(_object);
		_object = null;
		_roadObject = null;
		IsPlaceMode = false;
		_roadPlaceMode = false;
		RoadRemovalTexture = null;
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
		if (_object is not null)
		{
			GetParent().RemoveChild(_object);
		}
		GetParent().AddChild(building);
		_object = building;
		_containBuilding = false;
		IsPlaceMode = true;
		GameMenu.GameMode.Text = GameMode.Build;
	}

	private void PlaceRoad()
	{
		if (GameMenu.GameMode.Text == GameMode.RoadRemove)
		{	var gridPosition = _roadLayer.LocalToMap(GetGlobalMousePosition());
			if (_roadPositions.Contains(gridPosition))
			{
				_roadLayer.EraseCell(gridPosition);
				_roadPositions.Remove(gridPosition);
				_roadLayer.SetCellsTerrainConnect(_roadPositions, 0, 0);
				Resources[RawResource.Stone] += 1; // change to more generic method.
				_roadLayer.QueueRedraw();
			}
		}
		
		else if (CanPlaceRoad())
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
