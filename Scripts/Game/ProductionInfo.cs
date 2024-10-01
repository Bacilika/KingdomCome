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
		{"HunterLodge", ["Hunters Lodge", "1", "1", 
			"Station for hunters to gather and hunt. Produces meat."]},
		{"Well", ["Well", "1", "1", 
			"Allows citizens to get water."]},
		{"IronMine", ["IronMine", "1", "1", 
			"Mine for iron"]},
		{"MarketStall", ["MarketStall", "1", "1", 
			"Market stall to sell"]},
		{"StoneMine", ["StoneMine", "1", "1", 
			"Mine for producing stone"]},
		{"WoodCutter", ["WoodCutter", "1", "1", 
			"Produces wood"]},
		{"FarmHouse", ["WoodCutter", "1", "1", 
			"Produces wood"]}
	};
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("InfoBox/Title");
		_resources = GetNode<RichTextLabel>("InfoBox/Resources");
		_description = GetNode<RichTextLabel>("InfoBox/Description");
	}

	public void setInfo(AbstractPlaceable parent)
	{
		_title.Text = parent.GetBuildingName();
		_resources.Text = "Cost: Wood: " + _productionInfo[parent.GetBuildingName()][1] + " Stone: " + _productionInfo[parent.GetBuildingName()][2];
		_description.Text = _productionInfo[parent.GetBuildingName()][3];
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
