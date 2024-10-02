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
		{"HunterLodge", ["Hunters Lodge", 
			"Station for hunters to gather and hunt. Produces meat."]},
		{"Well", ["Well", 
			"Allows citizens to get water."]},
		{"IronMine", ["IronMine",  
			"Mine for iron"]},
		{"MarketStall", ["MarketStall", 
			"Market stall to sell"]},
		{"StoneMine", ["StoneMine",  
			"Mine for producing stone"]},
		{"WoodCutter", ["WoodCutter", 
			"Produces wood"]},
		{"FarmHouse", ["WoodCutter",  
			"Produces wood"]},
		{"House", ["House",
			"A place for citizens to live"]},
		{"Road", ["Road",
			"A road for citizens to walk on"]}
	};
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("InfoBox/Title");
		_resources = GetNode<RichTextLabel>("InfoBox/Resources");
		_description = GetNode<RichTextLabel>("InfoBox/Description");
	}

	public void setInfo(Node2D parent)
	{
		if (parent is AbstractPlaceable placeable)
		{
			_title.Text =  _productionInfo[placeable.GetBuildingName()][0];
			_resources.Text = $"Cost: {placeable.CostToString()}";
			_description.Text = _productionInfo[placeable.GetBuildingName()][1];
		}
		else //Road
		{
			_title.Text =  _productionInfo["Road"][0];
			_resources.Text = $"Cost: {GameLogistics.RoadPrice}";
			_description.Text = _productionInfo["Road"][1];
		}

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
