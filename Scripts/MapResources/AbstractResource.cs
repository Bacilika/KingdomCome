using System.Collections.Generic;
using Godot;

namespace KingdomCome.Scripts.MapResources;

public abstract partial class AbstractResource : Area2D
{
    public List<Npc> AssignedNpcs =[];
    public int ResourcesLeft = 0;

    public override void _Ready()
    {
    }
}