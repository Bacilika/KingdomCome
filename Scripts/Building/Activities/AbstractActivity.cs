using Scripts.Constants;

namespace KingdomCome.Scripts.Building.Activities;

public abstract partial class AbstractActivity : Production
{
	public bool IsOpen = true;
	public int maxVisitors = 10;
	protected override void Tick()
	{
		UpdateInfo();
	}

	protected override void OnDeleteInstance()
	{
		GameLogistics.Resources[RawResource.Unemployed] += GetWorkers();
	}

	public bool Visit()
	{
		if (CurrentPeople.Count >= maxVisitors)
		{
			return false;
		}
		return true;
	}
}
