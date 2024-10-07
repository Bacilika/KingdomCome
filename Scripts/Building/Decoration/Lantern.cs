using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class Lantern : AbstractPlaceable
{
	public int increaseHappiness = 1;
	
	public override void _Ready_instance()
	{
		Upgrades = new Dictionary<string, List<int>>
		{
		};
		BuildCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [5, 7, 10] },
			{ GameResource.Stone, [5, 7, 10] },
		};
		MoveCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [1, 2, 3] },
			{ GameResource.Stone, [1, 2, 3] },
		};
		DeleteCost = new Dictionary<string, List<int>>
		{
			{ GameResource.Wood, [2, 3, 4] },
			{ GameResource.Stone, [2, 3, 4] }
		};
		
	}

	public override void WhenShopReady(){}

	protected override void Tick()
	{
	}
	
	
	
	protected override void OnDeleteInstance()
	{
	}
}
