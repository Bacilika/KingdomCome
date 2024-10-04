using System;
using Scripts.Constants;

namespace KingdomCome.Scripts.Building.Activities;

public abstract partial class AbstractActivity: Production
{
	public abstract override void _Ready_instance();
	public abstract override void ProduceItem();
	public bool IsOpen = true; 
	protected override void Tick()
	{
		UpdateInfo();
	}
	protected override void OnDeleteInstance()
	{
		GameLogistics.Resources[GameResource.Unemployed] += GetWorkers();
	}
	
	
}
