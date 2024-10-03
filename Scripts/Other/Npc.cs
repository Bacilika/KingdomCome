using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public class MoodReason
{
	public string Reason { get; set; }
	public int Happiness { get; set; }
	
}

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
	public CitizenInfo Info;
	public Dictionary<string, MoodReason> moodReasons;

	private Vector2 _activityPosition;
	
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
		Info = GetNode<CitizenInfo>("CitizenInfo");
		Info.SetInfo(this);
		Info.GetNode<HBoxContainer>("HBoxContainer").Visible = false;
		Info.Visible = false;

		moodReasons = new()
		{
			{ "Work", new MoodReason() },
			{ "Activity", new MoodReason() }

		};
	}

	public void SetInfo()
	{
		Info.Position = Position;
		Info.Background.Visible = true;
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
		if (Info is not null && Info.Visible)
		{
			SetInfo();
		}
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
		var temphappiness = 5;
		foreach (var reason in moodReasons)
		{
			temphappiness += reason.Value.Happiness;
		}

		Happiness = temphappiness;
	}

	public String GetUnhappyReason()
	{
		string unhappyReason = "";

		if (moodReasons is not null)
		{
			foreach (var reason in moodReasons)
			{
				unhappyReason += reason.Value.Reason + "\n";
			}
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
			moodReasons["Work"].Reason = "Has work";
			moodReasons["Work"].Happiness = 1;
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

		if (_rnd.RandiRange(0, 3) == 0 && destination == workPosition && GameMap._placedActivities.Count > 0)
		{
			AbstractActivity activity = null;
			int counter = 0;
			do
			{
				var i = _rnd.RandiRange(0, GameMap._placedActivities.Count - 1);
				activity = GameMap._placedActivities[i];
				counter++;
			} while (!activity.IsOpen && counter < 5);

			if (activity.IsOpen)
			{
				destination = activity.Position;
				moodReasons["Activity"].Reason = "enjoys" + activity.BuildingName;
				moodReasons["Activity"].Happiness = 2;
			}
			else
			{
				destination = homePosition;
				moodReasons["Activity"].Reason = "No avaliable activity";
				moodReasons["Activity"].Happiness = -2;
			}
		}
		else
		{
			if (destination == workPosition || destination == _activityPosition)
			{
				destination = homePosition;
			}
			else
			{
				destination = workPosition;
			}

		}
	
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
			Info.Visible = !Info.Visible;
			Info.Position = Position;
			Info.SetInfo(this);
			
		}
		else if(@event.IsActionPressed(Inputs.LeftClick))
		{
			Info.Visible = false;
		}
		
	}
}
