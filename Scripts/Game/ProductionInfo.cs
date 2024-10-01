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
		{"HuntingButton", ["Hunters Lodge", "1", "1", 
			"Station for hunters to gather and hunt. Produces meat."]},
		{"WellButton", ["Well", "1", "1", 
			"Allows citizens to get water."]},
		{"IronMineButton", ["IronMine", "1", "1", 
			"Mine for iron"]},
		{"MarketStallButton", ["MarketStall", "1", "1", 
			"Market stall to sell"]},
		{"StoneButton", ["StoneMine", "1", "1", 
			"Mine for producing stone"]},
		{"WoodButton", ["WoodCutter", "1", "1", 
			"Produces wood"]}
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
		_resources.Text = "Cost: Wood: " + _productionInfo[parent.Name][1] + " Stone: " + _productionInfo[parent.Name][2];
		_description.Text = _productionInfo[parent.Name][3];
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
