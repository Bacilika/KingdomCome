using Godot;
using System;
using System.Collections.Generic;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class Npc : CharacterBody2D
{
	public House Home;
	public Production Work;
	private bool _focused;
	private bool isUnemployed = true;
	private NavigationAgent2D _navigation;
	private float _speed = 100;
	public Vector2 startPos;
	public int Happiness = 10;
	
	public Texture2D Sprite;
	private AnimatedSprite2D _animation;
	private Vector2 homePosition;
	private Vector2 workPosition;
	private Vector2 destination;
	private bool _ready;
	private Timer _timer;
	private bool timerOut;
	private RandomNumberGenerator _rnd = new ();
	private AudioStreamPlayer2D _walkingOnGrassSound;
	public CitizenInfo CitizenInfo;
	public HashSet<string> unhappyReasons = new();
	
	[Signal]
	public delegate void OnJobChangeEventHandler(Npc npc);
	
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Sprite = _animation.SpriteFrames.GetFrameTexture("walkUp", 0);
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_timer = GetNode<Timer>("WorkTimer");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
		destination = homePosition;
		CitizenInfo = GetNode<CitizenInfo>("CitizenInfo");
		CitizenInfo.SetInfo(this);
		CitizenInfo.GetNode<HBoxContainer>("HBoxContainer").Visible = false;
		CitizenInfo.Visible = false;
	}

	public void SetInfo()
	{
		CitizenInfo.Position = Position;
		CitizenInfo.Background.Visible = true;
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

	public override void _Process(double delta)
	{
		calculateHappiness();
	}

	
	public override void _PhysicsProcess(double delta)
	{
		if (_ready)
		{
			if (NavigationServer2D.MapGetIterationId(_navigation.GetNavigationMap()) == 0)
			{
				return;
			}

			if (_navigation.DistanceToTarget() < 10)
			{
				_animation.Stop();
				if (_timer.IsStopped())
				{
					_timer.Start();
					TurnOnAudio(false);
				}
				if (timerOut)
				{
					if (_rnd.RandiRange(0, 10)==0) //to make their movement a bit less monotone
					{
						setDestination();
						startPos = GetGlobalPosition();
						timerOut = false;
						_timer.Stop();
						var nextPos = _navigation.GetNextPathPosition();
						Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
						Velocity = new_vel;
						MoveAndSlide();
						_animation.Play();
						
						if(destination == homePosition) Work.GatherResource();
					}
				}

			}
			else
			{
				var nextPos = _navigation.GetNextPathPosition();
				Vector2 new_vel =  (GlobalPosition.DirectionTo(nextPos) * _speed);
				Velocity = new_vel;
				MoveAndSlide();
				_animation.Animation = GetDirection();
			}
		}
	}

	private string GetDirection()
	{

		var angle = Velocity.Angle();
		var angleToDegrees = angle * 180 / Math.PI;
		switch (angleToDegrees)
		{
			case >= 45 and < 135:
				return "walkUp";
			case >= -135 and < -45:
				return "walkDown";
			case >= 0 and < 45 or < 0 and >= -45:
				return "walkRight";
			default:
				return "walkLeft";
		}
	}

	public void calculateHappiness()
	{
		var temphappiness = 10;
		if (!IsEmployed())
		{
			temphappiness -= 5;
			unhappyReasons.Add("Unemployed (-5)");
		}
		Happiness = temphappiness;
	}

	public String GetUnhappyReason()
	{
		string unhappyReason = "";
		foreach (var i in unhappyReasons)
		{
			unhappyReason += i + "\n";
		}
		return unhappyReason;
	}

	public void SetStartPos(Vector2 pos)
	{
		startPos = pos;
		homePosition = startPos;
	}

	public bool GetJob(Production production, bool change = false)
	{
		if (Work is null || change)
		{

			if (Work is not null)
			{
				Work.RemoveWorker(this);
			}
			Work = production;
			//production.EmployWorker(this);
			workPosition = Work.Position;
			setDestination();
			return true;
		}

		return false;
	}
	

	public bool IsEmployed()
	{
		return Work != null;
	}

	public void OnDelete()
	{
		Work?.People.Remove(this);
		Home.People.Remove(this);
		QueueFree();
	}

	public void setDestination()
	{
		if (Home.hasMoved)
		{
			homePosition = Home.Position;
		}
		if (Work.hasMoved)
		{
			workPosition = Work.Position;
		}
		
		destination = destination == homePosition ? workPosition : homePosition;
	
		_navigation.SetTargetPosition(destination);
		_navigation.GetNextPathPosition();
		_ready = true;
		TurnOnAudio(true);
	}

	public void OnMouseEntered()
	{
		_focused = true;
	}

	public void OnMouseExited()
	{
		_focused = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (_focused && @event.IsActionPressed(Inputs.LeftClick))
		{
			CitizenInfo.Visible = !CitizenInfo.Visible;
			CitizenInfo.Position = Position;
			CitizenInfo.SetInfo(this);
			
		}
		else if(@event.IsActionPressed(Inputs.LeftClick))
		{
			CitizenInfo.Visible = false;
		}
		
	}
}
