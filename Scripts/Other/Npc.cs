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
	public LivingSpace Home;
	public Production Work;
	private bool _focused;
	private bool isUnemployed = true;
	private NavigationAgent2D _navigation;
	private float _speed = 100;
	public int Happiness = 10;
	private bool _still;
	
	public Texture2D Sprite;
	private AnimatedSprite2D _animation;
	private Vector2 destination;
	private bool _ready;
	private Timer _workTimer;
	private bool timerOut;
	private RandomNumberGenerator _rnd = new (); 
	private AudioStreamPlayer2D _walkingOnGrassSound;
	public CitizenInfo Info;
	public Dictionary<string, MoodReason> moodReasons;
	private bool _stopped;

	public AbstractPlaceable PlaceablePosition;
	private AbstractActivity activity;

	public Timer AtWorkTimer;

	private Vector2 _activityPosition;
	
	[Signal]
	public delegate void OnJobChangeEventHandler(Npc npc);
	
	[Signal]
	public delegate void OnAtWorkEventHandler(Npc npc);
	
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("WalkingAnimation");
		Sprite = _animation.SpriteFrames.GetFrameTexture("walkUp", 0);
		AtWorkTimer = GetNode<Timer>("AtWorkTimer");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_workTimer = GetNode<Timer>("WorkTimer");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
		//destination = Home.Position;
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

	public void OnWorkTimerTimeout()
	{
		timerOut = true;
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
		calculateHappiness();
		if (Info is not null && Info.Visible)
		{
			Info.SetInfo(this);
		}
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
			if (_workTimer.IsStopped()) //Reach Work
			{
				_workTimer.Start();
				ToggleWalkingSound(false);
				_animation.Animation = "work";
				
				if (PlaceablePosition == Work)
				{
					EmitSignal(SignalName.OnAtWork, this);
				}
			}
			if (timerOut) //done at work
			{
				if (_rnd.RandiRange(0, 10)==0) //to make their movement a bit less monotone
				{
					SwitchLocation();
					SetDestination();

					timerOut = false;
					_workTimer.Stop();
					if(destination == Home.Position) Work.GatherResource();
				}
			}

		}

	}

	public void Move()
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
				string happiness = reason.Value.Happiness.ToString();
				if (reason.Value.Happiness > 0)
				{
					happiness = $"+{reason.Value.Happiness}";
				}
				unhappyReason += $"{reason.Value.Reason} ({happiness}) \n";
			}
		}

		return unhappyReason;
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
			moodReasons["Work"].Reason = "Has work";
			moodReasons["Work"].Happiness = 1;
			if (!change)
			{
				SetDestination();
			}
			
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
		{
			Work.People.Remove(this);
			
		}
		else
		{
			GameLogistics.Resources[GameResource.Unemployed] -= 1;
		}
		QueueFree();
	}
	
	
	private void SwitchLocation()
	{
		if (PlaceablePosition is null)
		{
			PlaceablePosition = Home; 
		}
		switch (PlaceablePosition.BuildingName)
		{
			case var value when value == Home.BuildingName:
			{
				PlaceablePosition = Work;
				break; 
			}
			case var value when value == Work.BuildingName:
			{
				if (FindActivity() != null){
					PlaceablePosition = activity;
					moodReasons["Activity"].Reason = "enjoys" + activity.BuildingName;
					moodReasons["Activity"].Happiness = 2;
				}
				else
				{
					PlaceablePosition = Home;
					moodReasons["Activity"].Reason = "No avaliable activity";
					moodReasons["Activity"].Happiness = -2;
				}
				break; 
			}
			case var value when value == activity.BuildingName:
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
		{
			for (int i = 0; i < 5; i++)
			{
				var j = _rnd.RandiRange(0, GameMap._placedActivities.Count - 1);
				if (activity.IsOpen)
				{
					activity = GameMap._placedActivities[j];
					return activity;
				}
			}
		}
		return null;
	}
	
	public void SetDestination(Vector2 vec)
	{
		_navigation.SetTargetPosition(vec);
		_navigation.GetNextPathPosition();
	}

	public void SetDestination()
	{
		_navigation.SetTargetPosition(PlaceablePosition.Position);
		_navigation.GetNextPathPosition();
		_ready = true;
		ToggleWalkingSound(true);
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
		if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if (_focused && !_still || Info.focused)
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
