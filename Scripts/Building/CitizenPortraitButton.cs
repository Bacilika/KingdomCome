using Godot;
using System;

public partial class CitizenPortraitButton : Control
{
	public Npc npc;

	public PlaceableInfo InfoBox;
	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void OnNpcPressedEventHandler(Npc npc);

	public override void _Ready()
	{
		InfoBox = GetParent().GetParent().GetParent().GetParent<PlaceableInfo>();
		OnNpcPressed += InfoBox.ShowNpcInfo;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPortraitPressed()
	{
		EmitSignal(SignalName.OnNpcPressed, npc);

	}
	private void OnMouseEntered()
	{
		InfoBox.Focused = true;
		GrabFocus();
	}
	private void OnMouseExited()
	{
		ReleaseFocus();
		InfoBox.Focused = false;
		
	}
}
