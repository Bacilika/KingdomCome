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
	public int Timer { get; set; }
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
	
	[Signal]
	public delegate void OnHomelessNpcEventHandler(Npc npc);
	
	private AbstractActivity _activity;
	//npc fields
	public int Happiness = BaseHappiness;
	public string CitizenName = NameGenerator.GenerateFirstName();
	private AnimatedSprite2D _animation;
	private const int BaseHappiness = 5;
	private int Hunger = 0;
	private bool _focused;
	private int daysUnhappy = 0;
	private int daysNotFed = 0;
	public bool dead = false;
	
	private Dictionary<string, MoodReason> _moodReasons = new();

	//Navigation
	public NavigationAgent2D _navigation;
	private bool _stopped;
	public bool Idle = true;
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
	public AnimatedSprite2D Interaction;
	public bool isHomeless = true;

	private Vector2 _homelessPos = new (-1, -1);

	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("WalkingAnimation");
		Sprite = _animation.SpriteFrames.GetFrameTexture("walkDown", 0);
		AtWorkTimer = GetNode<Timer>("AtWorkTimer");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_walkingOnGrassSound = GetNode<AudioStreamPlayer2D>("GrassWalking");
		Info = GetNode<CitizenInfo>("CitizenInfo");
		Interaction = GetNode<AnimatedSprite2D>("Interaction");
		Info.SetInfo(this);
		Info.Visible = false;
		ZIndex = 1;
		
		if(isHomeless) EmitSignal(SignalName.OnHomelessNpc, this);
		
		ScheduleTimer = new Timer();
		ScheduleTimer.WaitTime = 30;
		ScheduleTimer.OneShot = true;
		ScheduleTimer.Timeout += () =>
		{
			AtWorkTimer.Stop();
			//ScheduleTimer.Stop();
			scheduleIsStarted = false;
			if (Work is WorkBench && Home == null)
			{
				ScheduleTimer.Start();
				return;
			}
			if (AtWork)
			{
				AtWork = false;
				Idle = true;
				CurrentBuilding = Work;
			}
			SetNextLocation();

		};
		ScheduleTimer.Autostart = false;
		AddChild(ScheduleTimer);
		_navigation.TargetReached += OnTargetReached;
		
		//Mood reasons.
		_moodReasons.Add("Work", new MoodReason());
		_moodReasons.Add("Activity", new MoodReason());
		_moodReasons.Add("Food", new MoodReason());
		_moodReasons.Add("Home", new MoodReason());
		_moodReasons.Add("Decoration", new MoodReason());
		_moodReasons.Add("Trauma", new MoodReason());
		SetMoodReason("Home", "Is Homeless", -3);
	}

	public void OnMoveIn()
	{
		SetMoodReason("Home", "Has a home", 0);
		isHomeless = false;
	}

	public void OnBuildingEntered(AbstractPlaceable building)
	{
		if (building != TargetBuilding) return;
		if (building is Production && building != _activity)
		{
			Work.SpaceOutWorkers();
		}
		if (AtWork) //citizen is working
		{
			Work.NpcWork(this); //do task
			return;
		}

		if (building == Work) // arrive at work 
		{
			AtWork = true;
			Idle = false;
			Work.SpaceOutWorkers();
		}
	}

	public void OnTargetReached()
	{
		_move = false;
		Idle = true;
		if (ScheduleTimer.IsStopped())
		{
			StartScheduleTimer();
		}
	}

	public void StartScheduleTimer()
	{
		scheduleIsStarted = true;
		_move = false;
		if (AtWork) // reach workplace
		{
			
			Work.NpcWork(this);
		}
		else
		{
			Idle = true;
		}
		ScheduleTimer.Start();
		CurrentBuilding = TargetBuilding;
		
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
		if (NavigationServer2D.MapGetIterationId(_navigation.GetNavigationMap()) == 0) return; //Not ready
		if(isHomeless) EmitSignal(SignalName.OnHomelessNpc, this);
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
			if (_navigation.DistanceToTarget() < 5)
			{
				_navigation.SetTargetPosition(Position);
			}
			
		}
		else //at destination
		{
			if (!Idle)
			{
				_animation.Play();
				_animation.Animation = $"work{_direction}";
				
			}
			
			else if (dead)
			{
				_animation.Animation = "dead";
				_animation.Play();
				
			}
			else
			{
				_animation.Play();
				_animation.Animation = "idle";
			}
			ToggleWalkingSound(false);

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
		Info.Position = Vector2.Zero;
		Info.MoveToFront();
	}

	private void CalculateHappiness()
	{
		Happiness = BaseHappiness;
		if (GameMap.GracePeriod)
		{
			return;
		}
		foreach (var entry in _moodReasons)
		{
			Happiness += entry.Value.Happiness;
		}

		if (Happiness < BaseHappiness)
		{
			Interaction.Animation = "ExclamationPoint";
			Interaction.Visible = true;
			Interaction.Play();
		}
		else
		{

			Interaction.Visible = false;
			Interaction.Stop();
		}
	}

	public string GetUnhappyReason()
	{
		if (GameMap.GracePeriod)
		{
			return "";
		}
		var sortedDict = from entry in _moodReasons orderby entry.Value.Happiness select entry;

		var unhappyReason = "";
		if (_moodReasons is not null)
			foreach (var entry in sortedDict)
			{
				if(entry.Value.Reason is null) continue;
				if (entry.Value.Happiness >= 0)
				{
					unhappyReason += $"+{entry.Value.Reason}\n";
				}
				else
				{
					unhappyReason += $"-{entry.Value.Reason}\n";
				}
			}
		return unhappyReason;
	}

	public bool ChangeJob(Production production)
	{
		var oldWork = Work;
		Work?.RemoveWorker(this);
		Work = production;
		if (TargetBuilding == oldWork)
		{
			
			SetDestinationToTargetBuilding(Work);
		}

		if (AtWork)
		{
			AtWork = false;
			SetDestination(Work.Position);
		}
		EmitSignal(SignalName.SendLog, $"{CitizenName} changed job from {oldWork.BuildingName} to {Work.BuildingName}");
		return true;
	}

	public void SetMoodReason(string type, string reason, int happiness, int timer = -1)
	{
		if (happiness == 0)
		{
			_moodReasons[type].Reason = "";
			_moodReasons[type].Happiness = 0;
		}
		_moodReasons[type].Reason = reason;
		_moodReasons[type].Happiness = happiness;
		_moodReasons[type].Timer = timer;
	}
	public bool GetJob(Production production)
	{
		if (production is null)
		{
			Work = null;
			Work?.RemoveWorker(this);
			SetMoodReason("Work", "Unemployed", -1);
			AtWorkTimer.Stop();
			ScheduleTimer.Stop();
			SetDestination(Home?.Position ?? new Vector2(-1,-1));
			
			
			return false;
		}
		if (Work is not null)
		{
			return ChangeJob(production);
		}
		Work = production;
		SetMoodReason("Work", "Has Work", 1);
		EmitSignal(SignalName.SendLog, $"{CitizenName} got their first job at the {Work.BuildingName}!");
		TutorialWindow.CompleteTutorialStep("Employ Npc");
		SetDestinationToTargetBuilding(Work);
		return true;
	}
	
	public bool IsEmployed()
	{
		return Work != null;
	}

	public void OnDelete()
	{
		EmitSignal(SignalName.SendLog, $"{CitizenName} is moving out");
		Work?.People.Remove(this);
		Home?.People.Remove(this);
		GetParent<GameMap>().Citizens.Remove(this);
		GetParent<GameMap>().RemoveChild(this);
		QueueFree();
	}
	
	private void SetNextLocation()
	{
		if (CurrentBuilding == null)
		{
			if (Work is not null)
			{
				SetDestinationToTargetBuilding(Work);
			}
			return;
		}
		
		switch (CurrentBuilding.GetBuildingName())
		{
			case var value when value == Home?.GetBuildingName():
				SetDestinationToTargetBuilding(Work);
				break;
			case var value when value == Work?.GetBuildingName():
				if (Home is null)
				{
					SetMoodReason("Home", "Is Homeless", -3);
					SetDestinationToTargetBuilding(Home);
					break;
					
				}
				if (!GoToActivity()) // Going home
				{
					SetDestinationToTargetBuilding(Home);
					break;
				}
				
				if (FindActivity() != null)
				{
					SetMoodReason("Activity", $"enjoys {_activity.BuildingName}", 2);
					EmitSignal(SignalName.SendLog, $"{CitizenName} enjoys spending time at the {_activity.BuildingName}");
					SetDestinationToTargetBuilding(_activity);
				}
				else
				{ 
					EmitSignal(SignalName.SendLog, $"{CitizenName} didn't find an activity");
					SetMoodReason("Activity", $"No available activity", -2);
					SetDestinationToTargetBuilding(Home);
				}
				break;
			case var value when value == _activity?.GetBuildingName():
				SetDestinationToTargetBuilding(Home);
				break;
			default:
				Console.WriteLine($"Trying to switch when currentBuilding is null, {CurrentBuilding?.GetBuildingName()}");
				break;
		}
	}

	private void SetDestinationToTargetBuilding(AbstractPlaceable target)
	{
		TargetBuilding = target;
		SetDestination(TargetBuilding is null ? _homelessPos : target.Position);
	}
	
	private bool GoToActivity()
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
		SetDestinationToTargetBuilding(Home);
		ScheduleTimer.Stop();
		AtWorkTimer.Stop();
		_move = true;
	}

	public void SetDestination(Vector2 vec)
	{
		_navigation.SetTargetPosition(vec);
		_navigation.GetNextPathPosition();
		_move = true;
		Idle = true;
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
		if (!@event.IsActionPressed(Inputs.LeftClick)) return;
		
		if (_focused || Info.focused)
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

	public void OnDayOver()
	{
		foreach (var entry in _moodReasons)
		{
			if (entry.Value.Timer > -1)
			{
				if (entry.Value.Timer == 0)
				{
					_moodReasons[entry.Key] = new MoodReason();
				}
				else
				{
					_moodReasons[entry.Key].Timer--;
				}
			}
		}
		if (GameLogistics.Resources[RawResource.Food] > 0)
		{
			GameLogistics.ConsumeFood();
			if (Hunger > 0)
			{
				Hunger--;
				EmitSignal(SignalName.OnFed, this, true);
				SetMoodReason("Food", "",0);
			}
		}
		else
		{
			Hunger++;
			if (Hunger >= 5)
			{
				EmitSignal(GameMap.SignalName.SendLog, $"{CitizenName} starved to death.");
				if (Home is not null){
					foreach (var familyMember in Home.People)
					{
						familyMember.SetMoodReason("Trauma", "Family member died", -3,3);
					}
				}
				AtWorkTimer.Stop();
				ScheduleTimer.Stop();
				_move = false;
				dead = true;
			}
			else
			{
				EmitSignal(SignalName.OnFed, this, false);
				SetMoodReason("Food", "Did not get fed",-Hunger);
			}
		}

		if (Happiness < 3)
		{
			daysUnhappy += 1;
			if (daysUnhappy > 5)
			{
				OnDelete();
				//Todo: Write in log that Npc has moved out. 
			}
		}
	}
}
