using Godot;
using System;
using Scripts.Constants;

public abstract partial class Production : AbstractPlaceable
{
	protected int Workers;
	private int _food;
	protected Timer _timer;
	public bool HasMaxEmployees;
	protected RandomNumberGenerator Rnd = new ();
	protected int ProductionRate = 10; // 1/ProductionRate % chance to produce item by 1 each tick. 

	protected override void Tick()
	{
		
		if (!HasMaxEmployees && GameLogistics.HasUnemployedCitizens())
		{
			if ( _timer is not null && _timer.IsStopped())
			{
				_timer.Start();
			}
		}
		UpdateInfo();
	}

	public void GatherResource()
	{
		ProduceItem();
		PlayAnimation();
	}

	public abstract void ProduceItem();
	public abstract override void _Ready_instance();


	[Signal]
	public delegate void LookingForWorkersEventHandler(Production production);
	

	public void EmployWorker()
	{
		Workers++;
		GameLogistics.WorkingCitizens++;
		if (Workers == Upgrades[Upgrade.MaxWorkers][Level])
		{
			HasMaxEmployees = true;
		}
	}
	
	public void OnFoodTimerTimeout()
	{
		_food++;
		GameLogistics.Food++;
		float time = 15 - Workers;
		_timer.Start(time);
	}
	
	protected override void OnDelete()
	{
		GameLogistics.WorkingCitizens -= Workers;
		//GameMenu.Money += Upgrades["MoneyBackOnDelete"][Level];
		GameLogistics.Wood += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Stone += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers;
	}
	public void PlayAnimation()
	{
		var animatedSprite = GetNode<AnimatedSprite2D>("Animation");
		animatedSprite.TextureRepeat = TextureRepeatEnum.Disabled;
		animatedSprite.Play();
		
	}


}
