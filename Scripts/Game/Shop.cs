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
	private int _roadPrice = 100;

	private List<AbstractShopIconContainer> _shopTabs;
	//Make overall level. If level > x, unlock certain buildings. 
	private bool _locked = true;
	private Control _productionInfo;

	[Signal]
	public delegate void OnBuildingButtonPressedEventHandler(AbstractPlaceable type);

	[Signal]
	public delegate void OnRoadBuildEventHandler(Road road);
	public override void _Ready()
	{


		placeAudio = GetNode<AudioStreamPlayer2D>("PlaceBuildingAudio");
		deleteAudio = GetNode<AudioStreamPlayer2D>("DeleteBuildingAudio");
		
		_shopTabs = [GetNode<AbstractShopIconContainer>("BuildTabButtons/Houses"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Productions"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Decorations"),
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Roads"), 
			GetNode<AbstractShopIconContainer>("BuildTabButtons/Activities")];
	}


	public override void _Process(double delta)
	{
	}
}
