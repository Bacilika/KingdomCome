using System.Collections.Generic;
using KingdomCome.Scripts.Building.Activities;
using Scripts.Constants;

public partial class Tavern : AbstractActivity
{
    protected override void _Ready_instance()
    {
        PlayerLevel = 2;
        BuildingName = "Tavern";
        BuildingDescription = "A place for citizens to gather and drink beer. Requires water and food";
        Upgrades = new Dictionary<string, List<int>>
        {
            { Upgrade.MaxWorkers, [5, 7, 10] }
        };
        BuildCost = new Dictionary<string, List<int>>
        {
            { GameResource.Wood, [5, 7, 10] },
            { GameResource.Stone, [5, 7, 10] }
        };
        MoveCost = new Dictionary<string, List<int>>
        {
            { GameResource.Wood, [1, 2, 3] },
            { GameResource.Stone, [1, 2, 3] }
        };
        DeleteCost = new Dictionary<string, List<int>>
        {
            { GameResource.Wood, [2, 3, 4] },
            { GameResource.Stone, [2, 3, 4] }
        };
    }

    public override void ProduceItem()
    {
        if (GameLogistics.Resources[GameResource.Food] > 0 && GameLogistics.Resources[GameResource.Water] > 0)
        {
            GameLogistics.Resources[GameResource.Food]--;
            GameLogistics.Resources[GameResource.Water]--;
            IsOpen = true;
        }
        else
        {
            IsOpen = false;
        }
    }
}