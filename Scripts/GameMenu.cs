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
	public static bool IsPlaceMode;
	public static bool ContainHouse;
	private int _money = 50000;
	public static int Citizens;
	public static int Happiness;
	public static int Food;
	public static int Stone;
	public static int WorkingCitizens; 

	private Godot.Collections.Dictionary<string, Label> _gameStatLabels;
	
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
			{"happiness",statLabels.GetNode<Label>("Happiness")}
		};
		var shop = GetNode<Shop>("MenuCanvasLayer/Container/Shop");
		shop.OnBuildingButtonPressed += BuildBuilding;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();

	}
	


	public override void _Input(InputEvent @event)
	{

		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if (_object != null)
			{
				if (CanPlace())
				{
					_money -= _object.GetBuildingPrice();
					var placedHouse = _object.Duplicate();
					EmitSignal(SignalName.HousePlaced, placedHouse);
				}
			}

		}
		else if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object?.QueueFree();
			_object = null;
			IsPlaceMode = false;
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
		return ContainHouse == false && _object.GetBuildingPrice() <= _money;
	}

	private void BuildBuilding(AbstractPlaceable house)
	{
		house.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(house);
		_object = house;
		IsPlaceMode = true;
	}

	

}
