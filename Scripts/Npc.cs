using Godot;
using System;

public partial class Npc : CharacterBody2D
{
	public House Home;
	public Production Work;
	private bool isUnemployed = true;
	private NavigationAgent2D _navigation;
	private float _speed = 100;
	public Vector2 startPos;
	private Vector2 homePosition;
	private Vector2 workPosition;
	private bool _ready;
	private Timer _timer;
	private bool timerOut = false;
	private RandomNumberGenerator _rnd = new ();
	private AudioStreamPlayer2D _walkingOnGrassSound;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
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
					if (_rnd.RandiRange(0, 10)==0) //to make their movement a bit less monotone
					{
						setDestination(startPos);
						startPos = GetGlobalPosition();
						timerOut = false;
						_timer.Stop();
						var nextPos = _navigation.GetNextPathPosition();
						Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
						Velocity = new_vel;
						MoveAndSlide();
						Work.ProduceItem();	
					}
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
		homePosition = startPos;
	}

	public void GetJob(Production production)
	{
		if (Work is null)
		{
			Work = production;
			production.EmployWorker();
			workPosition = Work.Position;
			setDestination(workPosition);
		}
	}

	public bool IsEmployed()
	{
		return Work != null;
	}

	public void setDestination(Vector2 destPos)
	{
		if (startPos == homePosition)
		{
			destPos = workPosition;
		}
		else
		{
			destPos = homePosition;
		}
		if (Home.hasMoved)
		{
			homePosition = Home.Position;
		}
		if (Work.hasMoved)
		{
			workPosition = Home.Position;
		}
		_navigation.SetTargetPosition(destPos);
		_navigation.GetNextPathPosition();
		_ready = true;
		TurnOnAudio(true);
	}
}
