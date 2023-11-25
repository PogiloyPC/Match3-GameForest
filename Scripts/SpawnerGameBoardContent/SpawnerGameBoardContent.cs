using Godot;

[GlobalClass]
public partial class SpawnerGameBoardContent : Resource
{
    [Export] private ConfigGameBoardContent _configContent;

    private Game _game;

    public void LoadSpawner(Game game)
    {
        _game = game;
    }

    public ElementGameBoard SpawnGameContent()
    {
        RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();

        int numberElement = randomNumberGenerator.RandiRange((int)TypeGameBoardContent.redColor, (int)TypeGameBoardContent.yellowColor);

        ElementGameBoard element = _configContent.GetElement((TypeGameBoardContent)numberElement);

        return element;
    }

    public ElementGameBoard SpawnEmptyElement() => _configContent.GetEmptyElement();

    public Destroyer SpawnDestroyer() => _configContent.GetDestroyer();

    public ElementGameBoard SpawnBonus(TypeBonusElement typeBonus)
    {
        switch (typeBonus)
        {
            case TypeBonusElement.bomb:
                return _configContent.GetBonusBomb();                
            case TypeBonusElement.horizontalLine:
                return _configContent.GetBonusHorizontalLine();                
            case TypeBonusElement.verticalLine:
                return _configContent.GetBonusVerticalLine();
            default:
                return null;
        }
    }

    public ElementGameBoard SpawnLineBonus() => _configContent.GetBonusHorizontalLine();
}