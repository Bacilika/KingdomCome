#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Scripts.Constants;

public partial class Shop : Control
{
	private GridContainer _buildButtons;
	public static AudioStreamPlayer2D placeAudio;
	public static AudioStreamPlayer2D deleteAudio;
	private bool _locked = true;
	private List<AbstractShopIconContainer> tabs;
	
	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);
	[Signal]
	public delegate void OnRoadBuildEventHandler(Road road);
	public override void _Ready()
	{
		placeAudio = GetNode<AudioStreamPlayer2D>("PlaceBuildingAudio");
		deleteAudio = GetNode<AudioStreamPlayer2D>("DeleteBuildingAudio");
		tabs = [
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Houses"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Productions"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Decorations"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Roads"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Activities"),
		];
	}

	public void OnTabClicked(int i)
	{
		tabs[i].UpdateStock(GameLogistics.Resources);
	}
}
