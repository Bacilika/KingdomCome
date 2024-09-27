using Godot;
using System;

public partial class ChooseWare : Control
{
	[Signal]
	public delegate void OnSellIronEventHandler();
	[Signal]
	public delegate void OnSellWoodEventHandler();
	[Signal]
	public delegate void OnSellStoneEventHandler();
	[Signal]
	public delegate void OnSellMeatEventHandler();
	[Signal]
	public delegate void OnSellWheatEventHandler();
	
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnSellIronButtonPressed()
	{
		EmitSignal(SignalName.OnSellIron);
	}
	public void OnSellMeatButtonPressed()
	{
		EmitSignal(SignalName.OnSellMeat);

	}
	public void OnSellWheatButtonPressed()
	{
		EmitSignal(SignalName.OnSellWheat);

	}
	public void OnSellStoneButtonPressed()
	{
		EmitSignal(SignalName.OnSellStone);

	}
	
	public void OnSellWoodButtonPressed()
	{
		EmitSignal(SignalName.OnSellWood);

	}
}
