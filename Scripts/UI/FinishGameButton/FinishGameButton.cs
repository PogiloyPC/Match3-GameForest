using Godot;

public partial class FinishGameButton : TextureButton
{
    public override void _Pressed()
    {
        base._Pressed();

        GetTree().ChangeSceneToFile("res://Scene/MainMenu.tscn");
    }
}
