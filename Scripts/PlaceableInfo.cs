using Godot;
using System;

public partial class PlaceableInfo : Control
{
	// Called when the node enters the scene tree for the first time.

	public bool Focused; 
	[Signal]
	public delegate void OnDeleteEventHandler();
	
	[Signal]
	public delegate void OnUpgradeEventHandler();
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
	

}
