using Godot;
using System;

public partial class TileBoard : Node2D
{
    public void SetPositionCell(Vector2I position) => Position = position;

    public Vector2 GetGlobalPosition() => GlobalPosition;
}