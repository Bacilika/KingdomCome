using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class ProductionInfo : Control
{
	private RichTextLabel _title;
	private RichTextLabel _resources;
	private RichTextLabel _description;
	private Dictionary<String, List<String>> _productionInfo = new()
	
	{
		{"HuntingButton", ["Hunters Lodge", //HunterLodge.Upgrades["WoodCost"][0].ToString(), HunterLodge.Upgrades["StoneCost"][0].ToString(), 
		"Station for hunters to gather and hunt. Produces meat."]}
	};
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("InfoBox/Title");
		_resources = GetNode<RichTextLabel>("InfoBox/Resources");
		_description = GetNode<RichTextLabel>("InfoBox/Description");
		setInfo();
	}

	public void setInfo()
	{
		var parent = GetParent();
		_title.Text = _productionInfo[parent.Name][0];
		//_resources.Text = "Wood: " + _productionInfo[parent.Name][1] + " Stone: " + _productionInfo[parent.Name][2];
		_description.Text = _productionInfo[parent.Name][1];
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
