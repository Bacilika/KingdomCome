using System;
using Godot;
using Scripts.Constants;

public partial class MenuBackground : TileMapLayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Root.MinSize =
            new Vector2I(ProjectSettings.GetSetting(Constants.ViewPortWidthSettingPath, 0).AsInt32(),
                ProjectSettings.GetSetting(Constants.ViewPortHeightSettingPath, 0).AsInt32());
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var scale = GetNode<MainMenu>("/root/Menu").CurrentScale;
        var largestScale = Math.Max(scale[0], scale[1]);
        Scale = new Vector2(largestScale, largestScale);
    }
}