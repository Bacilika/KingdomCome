using Godot;
using System;

public abstract partial class Production : AbstractPlaceable
{
	protected int Workers =0;

	[Signal]
	public delegate void LookingForWorkersEventHandler(Production production);
	// Called when the node enters the scene tree for the first time.
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
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	protected abstract override void OnDelete();


}
