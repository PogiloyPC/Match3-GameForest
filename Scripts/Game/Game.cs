using Godot;
using System;

public partial class Game : Node, ISeterSizeGameBoard, IGameTime, IGameScore
{
    [Export] private Grid _gameBoard;

    [Export] private UIController _UIController;

    private Action<IGameScore> _onChangeScore;
    private Action<IReward> _takeReward;
    private Action _gameOver;

    private IViewGameTime _viewGameTime;

    [Export] private Vector2I _sizeGameBoard;
    private Vector2I _touchFirstElement;
    private Vector2I _touchSecondElement;

    static private int _sizePixel = 60;
    private int _totalScore;

    [Export] private double _startGameTime;
    private double _currentGameTime;

    private bool _isFirstTouch = true;
    private bool _isSecondTouch;

    public override void _Ready()
    {
        _takeReward += TakeReward;

        _currentGameTime = _startGameTime;

        _UIController.InitController(out _viewGameTime, out _onChangeScore, out _gameOver);

        _gameBoard.LoadGameBoard(_takeReward, this, _sizePixel);
    }

    public override void _Process(double delta)
    {
        if (_currentGameTime <= 0)
        {
            GameOver();

            return;
        }

        TouchGrid();

        _currentGameTime -= delta;

        _viewGameTime.DisplayGameTime(this);
    }

    private void TouchGrid()
    {
        if (Input.IsActionJustPressed("LeftMouseButton") && _gameBoard.IsTouchable)
        {
            CheckTouchInGameBoard(GetViewport().GetMousePosition());
        }
    }

    private void CheckTouchInGameBoard(Vector2 mousePosition)
    {
        mousePosition = mousePosition - _gameBoard.Position;

        Vector2I gridMousePosition = new Vector2I();
        gridMousePosition.X = Mathf.RoundToInt(mousePosition.X / _sizePixel);
        gridMousePosition.Y = Mathf.RoundToInt(mousePosition.Y / _sizePixel);

        if (gridMousePosition.X >= 0 && gridMousePosition.Y >= 0)
        {
            if (gridMousePosition.X < _sizeGameBoard.X && gridMousePosition.Y < _sizeGameBoard.Y)
            {
                if (_isFirstTouch)
                {
                    _gameBoard.SelectTouchElement(gridMousePosition);

                    _touchFirstElement = gridMousePosition;

                    _isFirstTouch = false;
                }
                else
                {
                    _touchSecondElement = gridMousePosition;

                    _isSecondTouch = true;
                }
            }
        }

        if (_isSecondTouch)
        {
            _gameBoard.SwapElement(_touchFirstElement, _touchSecondElement);

            _isFirstTouch = true;
            _isSecondTouch = false;
        }
    }

    private void TakeReward(IReward reward)
    {
        _totalScore += reward.GetReward();

        _onChangeScore.Invoke(this);
    }

    private void GameOver() => _gameOver.Invoke();

    public int GetTotalScore() => _totalScore;

    public Vector2I GetSizeGameBoard() => _sizeGameBoard;

    public double GetCurrentGameTime() => _currentGameTime;

    public static int GetSizePixel() => _sizePixel;
}

