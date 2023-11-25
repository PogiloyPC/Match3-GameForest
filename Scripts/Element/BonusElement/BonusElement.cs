using Godot;

public partial class BonusElement : ElementGameBoard
{
    [Export] private TypeBonusElement _typeBonus;

    public void InitializeBonus(Color color, TypeGameBoardContent typeContent)
    {
        Modulate = color;

        _typeContent = typeContent;
    }

    public TypeBonusElement GetTypeBonus() => _typeBonus;
}

