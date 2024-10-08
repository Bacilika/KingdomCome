using Scripts.Constants;

namespace KingdomCome.Scripts.Building.Activities;

public abstract partial class AbstractActivity : Production
{
	public bool IsOpen = true;
	public abstract override void ProduceItem();

	protected override void Tick()
	{
		UpdateInfo();
	}

	protected override void OnDeleteInstance()
	{
		GameLogistics.Resources[GameResource.Unemployed] += GetWorkers();
	}
}
