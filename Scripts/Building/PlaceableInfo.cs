using Godot;
using System;
using System.Collections.Generic;

public partial class PlaceableInfo : Control
{
	
	public bool Focused;
	public PackedScene CitizenPortrait;
	public BoxContainer PortraitContainer;
	private CitizenInfo _citizenInfo;
	private Control _houseInfo;
	private RichTextLabel _textLabel;
	private Label _buildingName;
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
		_buildingName = GetNode<Label>("InfoBox/HouseInfo/BuildingName");
		_textLabel = GetNode<RichTextLabel>("InfoBox/HouseInfo/RichTextLabel");
		//TextLabel = GetNode<RichTextLabel>("InfoBox/HouseInfo/RichTextLabel");
		PortraitContainer = GetNode<BoxContainer>("InfoBox/HouseInfo/CitizenPortraitContainer");
		CitizenPortrait = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenPortraitButton.tscn");
		_citizenInfo = GetNode<CitizenInfo>("InfoBox/CitizenInfo");
		_citizenInfo.InfoBox = this;
		var citizenInfoBackground = GetNode<Control>("InfoBox/CitizenInfo/CitizenBackground");
		citizenInfoBackground.Visible = false;
		_houseInfo = GetNode<Control>("InfoBox/HouseInfo");

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
		_houseInfo.Visible = false;
		_citizenInfo.Visible = true;
		_citizenInfo.SetInfo(npc);
	}
	public void HideNpcInfo()
	{
		_houseInfo.Visible = true;
		_citizenInfo.Visible = false;
	}

	private void OnMouseEntered()
	{
		Focused = true;
	}
	private void OnMouseExited()
	{
		Focused = false;
		
	}

	public void UpdateInfo(string buildingName,string text)
	{
		_buildingName.Text = buildingName;
		_textLabel.Text = text;
	}
}
