using Godot;
using System;

public partial class Roads : AbstractShopIconContainer
{
	public override void AddProducts()
	{
		RoadProducts = [ResourceLoader.Load<PackedScene>("res://Scenes/Other/Road.tscn").Instantiate<Road>()];
	}
}
