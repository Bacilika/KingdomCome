using System.Data.SqlTypes;
using Godot;

namespace KingdomCome.Scripts.MapResources;

public abstract partial class AbstractResource: Area2D
{
    public int ResourcesLeft = 0;
    public override void _Ready()
    {
    }
    
}