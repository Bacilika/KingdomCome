using Godot;
using System;

public abstract partial class Production : AbstractPlaceable
{
	protected int Workers =0;
	private int _food;
	protected Timer _timer; 

	protected abstract override void Tick();
	public abstract override void _Ready_instance();


	[Signal]
	public delegate void LookingForWorkersEventHandler(Production production);
	
	public override void _ReadyProduction()
	{
		EmitSignal(SignalName.LookingForWorkers, this);
	}
	

	public void EmployWorker()
	{
		Workers++;
		if (Upgrades["MaxWorkers"][Level] > Workers)
		{
			EmitSignal(SignalName.LookingForWorkers, this);
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
		GameLogistics.Wood += Upgrades["WoodBackOnDelete"][Level];
		GameLogistics.Stone += Upgrades["StoneBackOnDelete"][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	public void UpdateInfo()
	{
		var textLabel = (RichTextLabel) InfoBox.GetChild(0).GetChild(0);
		textLabel.Text = "Workers: " + Workers;
	}


}
