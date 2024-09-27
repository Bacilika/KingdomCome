using Godot;
using System;

public partial class ChooseWare : Control
{
	public bool Focused;

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
	
	private void OnMouseEntered()
	{
		Focused = true;
	}
	private void OnMouseExited()
	{
		Focused = false;
		
	}
}