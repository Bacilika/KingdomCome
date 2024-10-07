using Godot;

public partial class ChooseWare : Control
{
	[Signal]
	public delegate void OnSellIronEventHandler();

	[Signal]
	public delegate void OnSellMeatEventHandler();

	[Signal]
	public delegate void OnSellStoneEventHandler();

	[Signal]
	public delegate void OnSellWheatEventHandler();

	[Signal]
	public delegate void OnSellWoodEventHandler();

	public bool Focused;


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
