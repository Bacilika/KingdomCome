using Godot;
using System;

public partial class Npc : CharacterBody2D
{
	public House Home;
	public AbstractPlaceable Work;
	private bool isUnemployed = true;
	private NavigationAgent2D _navigation;
	private float _speed = 100;
	public Vector2 startPos;
	private bool _ready;
	private Timer _timer;
	private bool timerOut = false;
	private AudioStreamPlayer2D _walkingOnGrassSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		Console.WriteLine();
		_timer = GetNode<Timer>("WorkTimer");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
	}

	public void OnWorkTimerTimeout()
	{
		timerOut = true;
	}

	private void TurnOnAudio(bool on)
	{
		if (on)
		{
			_walkingOnGrassSound.Play();
			return;
		}
		_walkingOnGrassSound.Stop();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_ready)
		{
			if (NavigationServer2D.MapGetIterationId(_navigation.GetNavigationMap()) == 0)
			{
				return;
			}

			if (_navigation.DistanceToTarget() < 1)
			{
				if (_timer.IsStopped())
				{
					_timer.Start();
					TurnOnAudio(false);
				}
				if (timerOut)
				{
					setDestination(startPos);
					startPos = GetGlobalPosition();
					timerOut = false;
					_timer.Stop();
					var nextPos = _navigation.GetNextPathPosition();
					Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
					Velocity = new_vel;
					MoveAndSlide();
				}

			}
			else
			{
				var nextPos = _navigation.GetNextPathPosition();
				Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
				Velocity = new_vel;
				MoveAndSlide();
			}
		}
	}

	public void SetStartPos(Vector2 pos)
	{
		startPos = pos;
	}

	public void GetJob(Production production)
	{
			Work = production;
			production.EmployWorker();
			setDestination(Work.Position);
	}

	public void setDestination(Vector2 destPos)
	{
		_navigation.SetTargetPosition(destPos);
		_navigation.GetNextPathPosition();
		_ready = true;
		TurnOnAudio(true);
	}
}
