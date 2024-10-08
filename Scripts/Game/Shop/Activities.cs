using Godot;

public partial class Activities : AbstractShopIconContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void AddProducts()
	{
		Products =
		[
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Activities/Church.tscn").Instantiate<Church>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Activities/Tavern.tscn").Instantiate<Tavern>()
		];
	}
}
