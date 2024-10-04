using Godot;
using System;

public abstract partial class LivingSpaces : AbstractPlaceable
{
	protected abstract override void Tick();
	public abstract override void _Ready_instance();
	
	
	protected override void OnDeleteInstance()
	{
	}
}
