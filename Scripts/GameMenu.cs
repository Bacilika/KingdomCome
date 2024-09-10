using Godot;
using System;
using System.Threading;
using Scripts.Constants;

public partial class GameMenu : Control
{
	// Called when the node enters the scene tree for the first time.

	private PackedScene _houseScene;
	private AbstractPlaceable _object;
	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		_houseScene = ResourceLoader.Load<PackedScene>("res://Scenes/House.tscn");
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void OnHouseButtonPressed()
	{
		Console.WriteLine("OnHouseButtonPressed");
		var house = _houseScene.Instantiate<House>();
		house.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(house);
		_object = house;
	}

	public override void _Input(InputEvent @event)
	{
		if (_object == null)
		{
			return;
		}

		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			var placedHouse =  _object.Duplicate();
			Console.WriteLine("emit signal");
			EmitSignal(SignalName.HousePlaced, placedHouse);
		}

		if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object.QueueFree();
			_object = null;
		}
	}
	
}
