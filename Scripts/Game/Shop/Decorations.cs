using Godot;

public partial class Decorations : AbstractShopIconContainer
{
	public override void AddProducts()
	{
		Products =
		[
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Flowerbed.tscn")
				.Instantiate<Flowerbed>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Lantern.tscn").Instantiate<Lantern>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Flower.tscn").Instantiate<Flower>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/Brush.tscn").Instantiate<Brush>(), 
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Decoration/FlowerVine.tscn").Instantiate<FlowerVine>(), 
		];
	}
}
