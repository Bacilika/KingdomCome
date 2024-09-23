using Godot;
using System;

public partial class Npc : CharacterBody2D
{
	private AbstractPlaceable _home;
	private NavigationAgent2D _navigation;
	private int _speed = 1;
	public Vector2 startPos;
	private bool _ready;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("Npc Ready");
		//_home = GetNode<AbstractPlaceable>("House");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		//Position = _home.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_ready)
		{
			Position = _navigation.GetNextPathPosition();
		}
		
		//Console.WriteLine(_navigation.GetNextPathPosition());
	}

	public void setDestination(Vector2 destPos)
	{
		_navigation.TargetPosition = destPos;
		Console.WriteLine("destpos" + _navigation.TargetPosition);

		_ready = true;
	}
	private void move()
	{
		
	}
}
