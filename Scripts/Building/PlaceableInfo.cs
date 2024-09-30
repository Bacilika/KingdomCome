using Godot;
using System;
using System.Collections.Generic;

public partial class PlaceableInfo : Control
{
	
	public bool Focused;
	public PackedScene CitizenPortrait;
	public BoxContainer PortraitContainer;
	public CitizenInfo CitizenInfo;
	public Control HouseInfo;
	public RichTextLabel TextLabel;
	public RichTextLabel TypeLabel;

	[Signal]
	public delegate void OnDeleteEventHandler();
	[Signal]
	public delegate void OnUpgradeEventHandler();
	[Signal]
	public delegate void OnMoveEventHandler();
	[Signal]	
	public delegate void OnChooseWareEventHandler();
	public override void _Process(double delta)
	{
		
	}

	public override void _Ready()
	{
		TextLabel = GetNode<RichTextLabel>("InfoBox/HouseInfo/RichTextLabel");
		TypeLabel = GetNode<RichTextLabel>("InfoBox/HouseInfo/Type");
		PortraitContainer = GetNode<BoxContainer>("InfoBox/HouseInfo/CitizenPortraitContainer");
		CitizenPortrait = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenPortraitButton.tscn");
		CitizenInfo = GetNode<CitizenInfo>("InfoBox/CitizenInfo");
		HouseInfo = GetNode<Control>("InfoBox/HouseInfo");

	}

	public void OnButtonDownPressed(){
				GD.Print("button down");
	}

	public void OnChooseWareButtonPressed()
	{
		EmitSignal(SignalName.OnChooseWare);

	}
	
	public void OnDeleteButtonPressed()
	{
		EmitSignal(SignalName.OnDelete); //emitted to AbstractPlaceable
	}

	public void OnUpgradeButtonPressed()
	{
		EmitSignal(SignalName.OnUpgrade); //emitted to AbstractPlaceable
	}
	
	public void OnMoveButtonPressed()
	{
		EmitSignal(SignalName.OnMove); //emitted to AbstractPlaceable
	}

	public void ShowNpcInfo(Npc npc)
	{
		HouseInfo.Visible = false;
		CitizenInfo.Visible = true;
		CitizenInfo.SetInfo(npc);
	}
	public void HideNpcInfo()
	{
		HouseInfo.Visible = true;
		CitizenInfo.Visible = false;
	}

	private void OnMouseEntered()
	{
		Focused = true;
	}
	private void OnMouseExited()
	{
		Focused = false;
		
	}

	public void UpdateInfo(string text, String type)
	{
		TypeLabel.Text = type;
		TextLabel.Text = text;
	}
}
