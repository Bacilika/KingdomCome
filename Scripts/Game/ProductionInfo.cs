using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;



public partial class ProductionInfo : PopupPanel
{
	private RichTextLabel _title;
	private RichTextLabel _resources;
	private RichTextLabel _description;
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("InfoBox/Title");
		_resources = GetNode<RichTextLabel>("InfoBox/Resources");
		_description = GetNode<RichTextLabel>("InfoBox/Description");
	}

	public void setInfo(AbstractPlaceable parent)
	{
		_title.Text =  parent.BuildingName;
		_resources.Text = $"Cost: {parent.CostToString()}";
		_description.Text = parent.BuildingDescription;


	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
