using Godot;
using System;

public partial class Houses : AbstractShopIconContainer
{
	public override void AddProducts()
	{
		Products = [ResourceLoader.Load<PackedScene>("res://Scenes/Building/LivingSpace/House.tscn").Instantiate<House>()];
	}
	
}