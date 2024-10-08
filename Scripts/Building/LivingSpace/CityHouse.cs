using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class CityHouse : LivingSpace
{
    private int _growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
    private Npc Npc;

    // Called when the node enters the scene tree for the first time.
    protected override void _Ready_instance()
    {
        PlayerLevel = 0;
        BuildingName = "City house";
        BuildingDescription = "A living space for more people but less comfortable.";
        Upgrades = new Dictionary<string, List<int>>
        {
            { Upgrade.MaxInhabitants, [10, 15, 20] }
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
    public Npc GetNpc()
    {
        return Npc;
    }
}