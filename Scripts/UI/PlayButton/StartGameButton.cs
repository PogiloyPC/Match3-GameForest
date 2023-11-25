using Godot;
using System;

public partial class StartGameButton : TextureButton
{
    public override void _Pressed()
    {
        base._Pressed();

        GetTree().ChangeSceneToFile("res://Scene/GameScene.tscn");
    }
}
