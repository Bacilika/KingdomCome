using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;



public partial class ProductionInfo : Panel
{
	private RichTextLabel _title;
	private RichTextLabel _resources;
	private RichTextLabel _description;
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("Title");
		_resources = GetNode<RichTextLabel>("Resources");
		_description = GetNode<RichTextLabel>("Description");
		
	}

	public void setInfo(AbstractPlaceable parent)
	{
		_title.Text =  parent.BuildingName;
		_resources.Text = $"Unlock at Level: {parent.PlayerLevel} \nCost: {parent.CostToString()}";
		_description.Text = parent.BuildingDescription;


	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
