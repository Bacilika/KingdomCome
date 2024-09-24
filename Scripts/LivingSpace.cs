using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	protected abstract override void OnDelete();
	// Called when the node enters the scene tree for the first time.
	
	public override void _ReadyProduction()
	{
	}

}
