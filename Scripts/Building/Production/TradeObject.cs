using Godot;
using System;

public partial class TradeObject : HBoxContainer
{
	public Label Label;
	public Label Value;
	public Button Minus;
	public Button Plus;
	public Label Amount;
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
		Value = GetNode<Label>("Value");
		Value.Text = "";
		Minus = GetNode<Button>("Minus");
		Plus = GetNode<Button>("Plus");
		Amount = GetNode<Label>("Amount");

		Minus.Pressed += () =>
		{
			var amountInt = int.Parse(Amount.Text);
			Amount.Text = Math.Max(amountInt-1,0).ToString();
		};
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
