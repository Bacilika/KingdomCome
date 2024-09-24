using Godot;
using System;

public partial class Npc : CharacterBody2D
{
	private AbstractPlaceable _home;
	private NavigationAgent2D _navigation;
	private float _speed = 100;
	public Vector2 startPos;
	private bool _ready;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("Npc Ready");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_ready)
		{
			if (NavigationServer2D.MapGetIterationId(_navigation.GetNavigationMap()) == 0)
			{
				return;
			}
			if (_navigation.IsNavigationFinished())
			{
				return;
			}
			var nextPos = _navigation.GetNextPathPosition();
			Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
			Velocity = new_vel;
			MoveAndSlide();

			//move();
		}
	}


	public void setDestination(Vector2 destPos)
	{
		_navigation.TargetPosition = destPos;
		_navigation.GetNextPathPosition();
		Console.WriteLine("Target" + _navigation.TargetPosition);
		Console.WriteLine("Dest" + destPos);
		Console.WriteLine("next pos"+_navigation.GetNextPathPosition());
		_ready = true;
	}
	private void move()
	{
		//Position = Position + Vector2.Down * _speed;
	}
}
