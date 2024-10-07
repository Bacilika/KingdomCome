using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Road : AbstractPlaceable
{
	protected override void Tick()
	{
		
	}
	public override void _Ready_instance()
	{
		BuildingName = "Road";
		BuildingDescription = "Road for citizens to walk on";
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Stone, [1, 1, 1] }
		};
	}
	protected override void OnDeleteInstance()
	{
	
	}
	
	public override void _Process(double delta)
	{
	}
}
