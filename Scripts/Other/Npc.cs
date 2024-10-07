using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public class MoodReason
{
	public string Reason { get; set; }
	public int Happiness { get; set; }
}

public partial class Npc : CharacterBody2D
{
	[Signal]
	public delegate void OnAtWorkEventHandler(Npc npc);

	[Signal]
	public delegate void OnJobChangeEventHandler(Npc npc);

	private const int BaseHappiness = 5;
	private AbstractActivity _activity;

	private Vector2 _activityPosition;
	private AnimatedSprite2D _animation;

	private Timer _dayTimer;
	private Vector2 _destination;
	private bool _focused;

	private Dictionary<string, MoodReason> _moodReasons;

	//Navigation
	private NavigationAgent2D _navigation;
	private bool _ready;
	private RandomNumberGenerator _rnd = new();
	private float _speed = 100;
	private bool _still;
	private bool _stopped;
	private bool _timerOut;
	private AudioStreamPlayer2D _walkingOnGrassSound;
	public Timer AtWorkTimer;

	public int Happiness = 10;
	public LivingSpace Home;
	public CitizenInfo Info;
	public AbstractPlaceable PlaceablePosition;
	public Texture2D Sprite;
	public Production Work;

	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("WalkingAnimation");
		Sprite = _animation.SpriteFrames.GetFrameTexture("walkUp", 0);
		AtWorkTimer = GetNode<Timer>("AtWorkTimer");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_dayTimer = GetNode<Timer>("WorkTimer");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
		Info = GetNode<CitizenInfo>("CitizenInfo");
		Info.SetInfo(this);
		Info.GetNode<HBoxContainer>("HBoxContainer").Visible = false;
		Info.Visible = false;


		_moodReasons = new Dictionary<string, MoodReason>
		{
			{ "Work", new MoodReason() },
			{ "Activity", new MoodReason() }
		};
	}

	public void OnWorkTimerTimeout()
	{
		_timerOut = true;
	}

	public void OnAtWorkTimerTimeout()
	{
		Work.AtWorkTimerTimeout(this);
		Console.WriteLine("Timeout");
	}


	private void ToggleWalkingSound(bool on)
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
		CalculateHappiness();
		if (Info is not null && Info.Visible) Info.SetInfo(this);
	}


	public override void _PhysicsProcess(double delta)
	{
		if (!_ready || NavigationServer2D.MapGetIterationId(_navigation.GetNavigationMap()) == 0) return; //Not ready

		if (_stopped) //stopped by player
		{
			_animation.Stop();
			ToggleWalkingSound(false);
			return;
		}

		if (_navigation.DistanceToTarget() > 10) //not at destination
		{
			_still = false;
			Move();
		}

		else //if at destination
		{
			if (_dayTimer.IsStopped()) //Reach Work
			{
				_dayTimer.Start();
				ToggleWalkingSound(false);
				_animation.Animation = "work";

				if (PlaceablePosition == Work) EmitSignal(SignalName.OnAtWork, this);
			}

			if (_timerOut) //done at work
				if (_rnd.RandiRange(0, 10) == 0) //to make their movement a bit less monotone
				{
					SwitchLocation();
					SetDestination();

					_timerOut = false;
					_dayTimer.Stop();
					if (_destination == Home.Position) Work.GatherResource();
				}
		}
	}

	private void Move()
	{
		Velocity = GlobalPosition.DirectionTo(_navigation.GetNextPathPosition()) * _speed;
		_animation.Animation = GetDirection();
		MoveAndSlide();
		_animation.Play();
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

	public void ShowInfo()
	{
		Info.SetInfo(this);
		Info.Visible = true;
	}

	private void CalculateHappiness()
	{
		Happiness = BaseHappiness + _moodReasons.Sum(reason => reason.Value.Happiness);
	}

	public string GetUnhappyReason()
	{
		var unhappyReason = "";

		if (_moodReasons is not null)
			foreach (var reason in _moodReasons)
			{
				var happiness = reason.Value.Happiness.ToString();
				if (reason.Value.Happiness > 0) happiness = $"+{reason.Value.Happiness}";
				unhappyReason += $"{reason.Value.Reason} ({happiness}) \n";
			}

		return unhappyReason;
	}


	public bool GetJob(Production production, bool change = false)
	{
		if (Work is null || change)
		{
			Work?.RemoveWorker(this);
			Work = production;
			_moodReasons["Work"].Reason = "Has work";
			_moodReasons["Work"].Happiness = 1;
			if (!change) //if this is first job, go to work!
				SetDestination();
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
		GameLogistics.Resources[GameResource.Citizens] -= 1;
		if (Work is not null)
			Work.People.Remove(this);
		else
			GameLogistics.Resources[GameResource.Unemployed] -= 1;
		QueueFree();
	}


	private void SwitchLocation()
	{
		PlaceablePosition ??= Home;
		switch (PlaceablePosition.BuildingName)
		{
			case var value when value == Home.BuildingName:
			{
				PlaceablePosition = Work;
				break;
			}
			case var value when value == Work.BuildingName:
			{
				if (FindActivity() != null)
				{
					PlaceablePosition = _activity;
					_moodReasons["Activity"].Reason = "enjoys" + _activity.BuildingName;
					_moodReasons["Activity"].Happiness = 2;
				}
				else
				{
					PlaceablePosition = Home;
					_moodReasons["Activity"].Reason = "No avaliable activity";
					_moodReasons["Activity"].Happiness = -2;
				}

				break;
			}
			case var value when value == _activity.BuildingName:
			{
				PlaceablePosition = Home;
				break;
			}
			default:
			{
				Console.WriteLine("Unknown building name");
				break;
			}
		}
	}

	private AbstractActivity FindActivity()
	{
		if (_rnd.RandiRange(0, 3) == 0 && GameMap._placedActivities.Count > 0)
			for (var i = 0; i < 5; i++)
			{
				var j = _rnd.RandiRange(0, GameMap._placedActivities.Count - 1);
				if (_activity.IsOpen)
				{
					_activity = GameMap._placedActivities[j];
					return _activity;
				}
			}

		return null;
	}

	public void SetDestination(Vector2 vec)
	{
		_navigation.SetTargetPosition(vec);
		_navigation.GetNextPathPosition();
	}

	private void SetDestination()
	{
		_navigation.SetTargetPosition(PlaceablePosition.Position);
		_navigation.GetNextPathPosition();
		_ready = true;
		ToggleWalkingSound(true);
	}

	private void OnMouseEntered()
	{
		_focused = true;
	}

	private void OnMouseExited()
	{
		_focused = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if ((_focused && !_still) || Info.focused)
			{
				ShowInfo();
				_stopped = true;
			}
			else
			{
				Info.Visible = false;
				_stopped = false;
			}
		}
	}
}
