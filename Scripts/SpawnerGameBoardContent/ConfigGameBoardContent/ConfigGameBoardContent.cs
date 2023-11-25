using Godot;

[GlobalClass]
public partial class ConfigGameBoardContent : Resource
{
    [Export]
    private string _moneyPath, _moonPath, _rectanglePath, _rhombusPath, _starPath, _emptyPath, _bonusBombPath,
        _bonusHorizontalLinePath, _bonusVerticalLinePath;
    [Export] private string _destroyerPath;

    public ElementGameBoard GetElement(TypeGameBoardContent typeElement)
    {
        switch (typeElement)
        {
            case (TypeGameBoardContent.yellowColor):
                return GetInstance(_moneyPath);
            case (TypeGameBoardContent.beigeColor):
                return GetInstance(_moonPath);
            case (TypeGameBoardContent.redColor):
                return GetInstance(_rectanglePath);
            case (TypeGameBoardContent.purpleColor):
                return GetInstance(_rhombusPath);
            case (TypeGameBoardContent.blueColor):
                return GetInstance(_starPath);
            default:
                return null;
        }
    }

    public ElementGameBoard GetEmptyElement() => GetInstance(_emptyPath);

    public ElementGameBoard GetBonusBomb() => GetInstance(_bonusBombPath);

    public ElementGameBoard GetBonusHorizontalLine() => GetInstance(_bonusHorizontalLinePath);

    public ElementGameBoard GetBonusVerticalLine() => GetInstance(_bonusVerticalLinePath);

    public Destroyer GetDestroyer()
    {
        PackedScene packedDestroyer = GD.Load(_destroyerPath) as PackedScene;

        if (packedDestroyer.Instantiate() is Destroyer destroyer)
        {
            return destroyer;
        }

        return null;
    }

    private ElementGameBoard GetInstance(string path)
    {
        PackedScene packed = GD.Load(path) as PackedScene;

        if (packed.Instantiate() is ElementGameBoard element)
            return element;
        else
            return null;
    }
}

