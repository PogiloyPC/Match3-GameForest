using Godot;

public interface ISeterSizeGameBoard
{
    Vector2I GetSizeGameBoard();
}

public interface IGameTime
{
    double GetCurrentGameTime();
}

public interface IGameScore
{
    int GetTotalScore();
}