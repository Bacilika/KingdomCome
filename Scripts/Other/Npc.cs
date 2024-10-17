using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Building.Activities;
using KingdomCome.Scripts.Other;
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
	[Signal]
	public delegate void SendLogEventHandler(string log);
	
	[Signal]
	public delegate void OnFedEventHandler(Npc npc, bool fed);
	
	private const int BaseHappiness = 5;
	private AbstractActivity _activity;
	private Vector2 _activityPosition;
	private Vector2 _destination;
	private bool _focused;
	
	//npc fields
	public int Happiness = BaseHappiness;
	public string CitizenName = NameGenerator.GenerateFirstName();
	private AnimatedSprite2D _animation;
	public int Hunger = 0;

	
	private Dictionary<string, MoodReason> _moodReasons;

	//Navigation
	private NavigationAgent2D _navigation;
	private bool _ready;
	private bool _still;
	private bool _stopped;
	private RandomNumberGenerator _rnd = new();
	private float _speed = 100;
	private AudioStreamPlayer2D _walkingOnGrassSound;
	public Timer AtWorkTimer;
	public LivingSpace Home;
	public CitizenInfo Info;
	public AbstractPlaceable PlaceablePosition;
	public Texture2D Sprite;
	public Production Work;
	public AbstractPlaceable CurrentBuilding;
	public Timer ScheduleTimer;
	public bool _move;
	public AbstractPlaceable TargetBuilding;
	public bool AtWork;
	public double time;
	public string _direction;
	private bool scheduleIsStarted = false;

	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("WalkingAnimation");
		Sprite = _animation.SpriteFrames.GetFrameTexture("walkDown", 0);
		AtWorkTimer = GetNode<Timer>("AtWorkTimer");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
		Info = GetNode<CitizenInfo>("CitizenInfo");
		Info.SetInfo(this);
		Info.GetNode<HBoxContainer>("HBoxContainer").Visible = true;
		Info.Visible = false;
		ScheduleTimer = new Timer();
		ScheduleTimer.WaitTime = 30;
		ScheduleTimer.OneShot = true;
		ScheduleTimer.Timeout += () =>
		{
			AtWorkTimer.Stop();
			//ScheduleTimer.Stop();
			scheduleIsStarted = false;
			if (AtWork)
			{
				AtWork = false;
				CurrentBuilding = Work;
				SetNextLocation();
			}
			_move = true;
		};
		ScheduleTimer.Autostart = false;
		AddChild(ScheduleTimer);
		_navigation.TargetReached += () =>
		{
			Console.WriteLine("Target Reached");
			if(!scheduleIsStarted)
				StartScheduleTimer();
		};
		
		_moodReasons = new Dictionary<string, MoodReason>
		{
			{ "Work", new MoodReason() },
			{ "Activity", new MoodReason() }, 
			{"Food", new MoodReason()}
		};
	}

	public void OnBuildingEntered(AbstractPlaceable building)
	{
		if (building != TargetBuilding)
		{
			return;
		}
		if (!ScheduleTimer.IsStopped()) //citizen is working
		{
			Work.NpcWork(this); //do task
			return;
		}
		
		if (!building.ActivityIndoors)
		{
			_navigation.SetTargetPosition(Position);
		}
		
	}

	public void StartScheduleTimer()
	{
		scheduleIsStarted = true;
		_move = false;
		if (CurrentBuilding == null && TargetBuilding == Work) // reach workplace
		{
			AtWork = true;
			Work.NpcWork(this);
		}
		ScheduleTimer.Start();
		CurrentBuilding = TargetBuilding;
		if (!AtWork)
		{
			SetNextLocation();
		}
		
	}
	public void OnBuildingExited()
	{
		CurrentBuilding = null;
	}

	public void OnAtWorkTimerTimeout()
	{
		Work.AtWorkTimerTimeout(this);
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

		if (_move) // going to destination
		{
			_direction = GetDirection();
			if (!_walkingOnGrassSound.Playing)
			{
				ToggleWalkingSound(true);
			}

			Move();
			if (_navigation.DistanceToTarget() < 1)
			{
				_navigation.SetTargetPosition(Position);
			}
		}
		else //at destination
		{
			ToggleWalkingSound(false);
			_animation.Animation = $"work{_direction}";
		}
	}

	private void Move()
	{
		Velocity = GlobalPosition.DirectionTo(_navigation.GetNextPathPosition()) * _speed;
		_animation.Animation = $"walk{_direction}";
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
				return "Down";
			case >= -135 and < -45:
				return "Up";
			case >= 0 and < 45 or < 0 and >= -45:
				return "Right";
			default:
				return "Left";
		}
	}

	public void ShowInfo()
	{
		Info.SetInfo(this);
		Info.Visible = true;
	}

	private void CalculateHappiness()
	{
		Happiness = BaseHappiness;
		foreach (var reason in _moodReasons)
		{
			Happiness += reason.Value.Happiness;
		}

		if (Happiness < BaseHappiness)
		{
			GetNode<Sprite2D>("ExclamationPoint").Visible = true;
		}
		else
		{
			GetNode<Sprite2D>("ExclamationPoint").Visible = false;
		}
	}

	public string GetUnhappyReason()
	{
		var unhappyReason = "";

		if (_moodReasons is not null)
			foreach (var reason in _moodReasons)
			{
				if(reason.Value.Reason is null) continue;
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
			var oldWork = Work;
			Work?.RemoveWorker(this);
			Work = production;
			_moodReasons["Work"].Reason = "Has work";
			_moodReasons["Work"].Happiness = 1;
			if (!change) //if this is first job, go to work!
			{
				EmitSignal(SignalName.SendLog, $"{CitizenName} got their first job at the {Work.BuildingName}!");
				_move = true;
				SetDestination(Work.Position);
				TargetBuilding = Work;
				
			}
			else
			{
				AtWork = false;
				if (oldWork is not null)
				{
					EmitSignal(SignalName.SendLog, $"{CitizenName} changed job from {oldWork.BuildingName} to {Work.BuildingName}");
				}
				else
				{
					EmitSignal(SignalName.SendLog, $"{CitizenName} got their first job at the {Work.BuildingName}!");
					TargetBuilding = Work;
				}
				
			}
			_ready = true;
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
		EmitSignal(SignalName.SendLog, $"{CitizenName} is moving out");
		Work?.People.Remove(this);
		Home.People.Remove(this);
		GameLogistics.Resources[RawResource.Citizens] -= 1;
		if (Work is not null)
			Work.People.Remove(this);
		else
			if (GameLogistics.Resources[RawResource.Unemployed] > 0)
			GameLogistics.Resources[RawResource.Unemployed] -= 1;
		QueueFree();
	}


	public void SetNextLocation()
	{
		if (CurrentBuilding == null)
		{
			return;
		}
		switch (CurrentBuilding.GetBuildingName())
		{
			case var value when value == Home?.GetBuildingName():
			{
				if (Work == null)
				{
					SetDestination(GlobalPosition);
					break;
				}
				TargetBuilding = Work;
				SetDestination(Work.Position);
				break;
			}
			case var value when value == Work?.GetBuildingName():
			{
				if (!GoToActivity()) // Going home
				{
					TargetBuilding = Home;
					if (Home is null)
					{
						SetDestination(new Vector2(0,0));
						break;
					}
					SetDestination(Home.Position);
					break;
				}
				
				if (FindActivity() != null)
				{
					_moodReasons["Activity"].Reason = "enjoys" + _activity.BuildingName;
					_moodReasons["Activity"].Happiness = 2;
					EmitSignal(SignalName.SendLog, $"{CitizenName} enjoys spending time at the {_activity.BuildingName}");
					TargetBuilding = _activity;
					SetDestination(_activity.Position);
				}
				else
				{ 
					TargetBuilding = Home;
					EmitSignal(SignalName.SendLog, $"{CitizenName} didn't find an activity");
					_moodReasons["Activity"].Reason = "No available activity";
					_moodReasons["Activity"].Happiness = -2;
					if (Home is null)
					{
						SetDestination(new Vector2(0,0));
						break;
					}
					SetDestination(Home.Position);
				}

				break;
			}
			case var value when value == _activity?.GetBuildingName():
			{
				TargetBuilding = Home;
				SetDestination(Home.Position);
				break;
			}
			default:
			{
				Console.WriteLine("Trying to switch when currentBuilding is null");
				throw new Exception();
			}
		}
	}
	public bool GoToActivity()
	{
		return _rnd.RandiRange(0, 3) == 0;
	}

	private AbstractActivity FindActivity()
	{
		if (GameMap._placedActivities.Count > 0)
			for (var i = 0; i < 5; i++)
			{
				var j = _rnd.RandiRange(0, GameMap._placedActivities.Count - 1);
				_activity = GameMap._placedActivities[j];
				if (_activity.IsOpen && _activity.Visit())
				{
					return _activity;
				}
			}
		return null;
	}

	public void OnWorkDelete()
	{
		Work = null;
		AtWork = false;		
		_animation.Stop();
		TargetBuilding = Home;
		SetDestination(Home.Position);
		ScheduleTimer.Stop();
		AtWorkTimer.Stop();
		_move = true;
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

	public void OnDayOver()
	{
		if (GameLogistics.Resources[RawResource.Food] > 0)
		{
			GameLogistics.Resources[RawResource.Food]--;
			if (Hunger > 0)
			{
				Hunger--;
				EmitSignal(SignalName.OnFed, this, true);
				_moodReasons["Food"].Reason = "Got fed";
				_moodReasons["Activity"].Happiness = 0;
			}
		}
		else
		{
			EmitSignal(SignalName.OnFed, this, false);
			_moodReasons["Food"].Reason = "Did not get fed";
			_moodReasons["Food"].Happiness = -2;
		}
	}
}
