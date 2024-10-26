#nullable enable
using System;
using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.Building;

public partial class Shop : Control
{
	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);

	[Signal]
	public delegate void OnRoadBuildEventHandler(Road road);
	[Signal]
	public delegate void OnRoadRemoveEventHandler(Sprite2D sprite2D);
	
	public static AudioStreamPlayer2D placeAudio;
	public static AudioStreamPlayer2D deleteAudio;
	private GridContainer _buildButtons;
	private bool _locked = true;
	private List<AbstractShopIconContainer> tabs;

	public override void _Ready()
	{
		placeAudio = GetNode<AudioStreamPlayer2D>("PlaceBuildingAudio");
		deleteAudio = GetNode<AudioStreamPlayer2D>("DeleteBuildingAudio");
		tabs =
		[
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Houses"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Productions"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Decorations"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Roads"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Activities")
		];

	}

	public void OnTabClicked(int i)
	{
		tabs[i].UpdateStock(GameLogistics.Resources);
	}
}
