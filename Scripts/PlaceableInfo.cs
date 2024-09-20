using Godot;
using System;

public partial class PlaceableInfo : Control
{
	
	public bool Focused; 
	[Signal]
	public delegate void OnDeleteEventHandler();
	[Signal]
	public delegate void OnUpgradeEventHandler();
	public override void _Process(double delta)
	{
	}
	
	public void OnButtonDownPressed(){
				GD.Print("button down");
	}
	
	public void OnDeleteButtonPressed()
	{
		EmitSignal(SignalName.OnDelete);
	}

	public void OnUpgradeButtonPressed()
	{
		EmitSignal(SignalName.OnUpgrade);

	}

	private void OnMouseEntered()
	{
		Focused = true;
	}
	private void OnMouseExited()
	{
		Focused = false;
		
	}
	

}
