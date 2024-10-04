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
	protected override void OnDelete()
	{
		for (int i = People.Count-1; i > 0; i--)
		{
			var npc = People[i];
			npc.OnDelete();
		}
		GameLogistics.Resources["UnEmployed"] += GetWorkers();
		GameLogistics.Resources["Wood"] += Upgrades[Upgrade.WoodBackOnDelete][Level];
		GameLogistics.Resources["Stone"] += Upgrades[Upgrade.StoneBackOnDelete][Level];
		Shop.deleteAudio.Play();
		QueueFree();
	}
	
	
}
