using Godot;
using System;

public partial class CitizenPortraitButton : Control
{
	public Npc npc;
	public PlaceableInfo InfoBox;
	public override void _Ready()
	{
		InfoBox = GetParent().GetParent().GetParent().GetParent<PlaceableInfo>();
	}
	
	public void OnPortraitPressed()
	{
		npc.ShowInfo();
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
