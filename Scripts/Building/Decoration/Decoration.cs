using System.Collections.Generic;

namespace KingdomCome.Scripts.Building.Decoration;

public abstract partial class Decoration: AbstractPlaceable
{
    public int DecorationsPoint = 0;
    public int increaseHappiness = 0;
    
    protected override void Tick()
    {
        UpdateInfo();
    }
    public void addDecorationPoint(List<Npc> npcs)
    {
        if (increaseHappiness < 2) increaseHappiness += 1;
        foreach (var npc in npcs)
        {
            npc.SetMoodReason("Decoration", "Nicely decorated", increaseHappiness);
        }
    }
    
    public void UpdateInfo()
    {
        InfoBox.UpdateInfo($"{BuildingName}",
            $"{BuildingDescription}");
    }
    
}