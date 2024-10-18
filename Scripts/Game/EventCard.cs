using Godot;
using System;

public partial class EventCard : Control
{

	[Signal]
	public delegate void OnEventDoneEventHandler(EventCard card);
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
