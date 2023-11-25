using Godot;
using System;

public partial class UIController : Control
{
    [Export] private ViewGameTime _gameTimer;
    [Export] private ViewTotalScore _viewTotalScore;

    [Export] private PanelGameOver _panelGameOver;

    private Action<IGameScore> _onChangeScore;
    private Action _gameOver;

    public override void _EnterTree()
    {
        base._EnterTree();

        _onChangeScore += _viewTotalScore.DisplayTotalScore;
        _gameOver += DisplayGameOver;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        _onChangeScore -= _viewTotalScore.DisplayTotalScore;
        _gameOver -= DisplayGameOver;
    }

    public void InitController(out IViewGameTime gameTimer, out Action<IGameScore> onChangeScore, out Action gameOver)
    {
        _panelGameOver.HidePanel();

        gameTimer = _gameTimer;

        onChangeScore = _onChangeScore;

        gameOver = _gameOver;
    }

    private void DisplayGameOver() => _panelGameOver.ShowPanel();
}

