using Scripts.Constants;

namespace KingdomCome.Scripts.Building.Activities;

public abstract partial class AbstractActivity : Production
{
    public bool IsOpen = true;
    public int maxVisitors = 10;
    public int visitors;
    
    public abstract override void ProduceItem();
    protected override void Tick()
    {
        UpdateInfo();
    }

    protected override void OnDeleteInstance()
    {
        GameLogistics.Resources[GameResource.Unemployed] += GetWorkers();
    }

    public bool Visit()
    {
        if (visitors >= maxVisitors)
        {
            return false;
        }

        visitors++;
        return true;
    }
}