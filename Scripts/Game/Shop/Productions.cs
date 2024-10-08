using Godot;

public partial class Productions : AbstractShopIconContainer
{
	public override void AddProducts()
	{
		Products =
		[
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/FarmHouse.tscn")
				.Instantiate<FarmHouse>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/HunterLodge.tscn")
				.Instantiate<HunterLodge>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/IronMine.tscn").Instantiate<IronMine>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/MarketStall.tscn")
				.Instantiate<MarketStall>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/StoneMine.tscn")
				.Instantiate<StoneMine>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/Well.tscn").Instantiate<Well>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/WoodCutter.tscn")
				.Instantiate<WoodCutter>(), 
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/Windmill.tscn")
			.Instantiate<Windmill>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/Bakery.tscn")
			.Instantiate<Bakery>(), 
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/Brewery.tscn")
				.Instantiate<Brewery>(),
			ResourceLoader.Load<PackedScene>("res://Scenes/Building/Production/SawMill.tscn")
			.Instantiate<SawMill>()
			
		];
	}
}
