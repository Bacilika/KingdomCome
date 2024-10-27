using Godot;
using System;

public partial class TradeObject : HBoxContainer
{
	public Label Label;
	public Label ResourcesOwned;
	public Button Minus;
	public Button Plus;
	public Label AmountToTrade;
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
		ResourcesOwned = GetNode<Label>("Value");
		ResourcesOwned.Text = "";
		Minus = GetNode<Button>("Minus");
		Plus = GetNode<Button>("Plus");
		AmountToTrade = GetNode<Label>("Amount");

		
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
