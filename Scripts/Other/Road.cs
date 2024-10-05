using Godot;
using System;

public partial class Road : AbstractPlaceable
{
	protected override void Tick()
	{
		
	}
	public override void _Ready_instance()
	{
		Price = 100;
	}
	protected override void OnDeleteInstance()
	{
	
	}
	public override void _Ready()
	{
		BuildingName = "Road";
		BuildingDescription = "Road for citizens to walk on";
	}


	public override void _Process(double delta)
	{
	}
}
