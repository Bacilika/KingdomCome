using Godot;
using System;

public partial class Decorations : AbstractShopIconContainer
{
	
	public override void AddProducts()
	{
		Products = [ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Flowerbed.tscn").Instantiate<Flowerbed>(), 
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Lantern.tscn").Instantiate<Lantern>()
			];

	}
}
