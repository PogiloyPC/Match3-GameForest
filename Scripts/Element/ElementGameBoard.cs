using Godot;

public partial class ElementGameBoard : Node2D, IGameBoardContent, IReward
{
    [Export] protected TypeGameBoardContent _typeContent;

    private Tween _tweenMove;

    private Vector2I _cordinatsInGrid;

    [Export] private Color _color;

    [Export] private float _speed;

    private int _reward = 1;

    public Color GetColorElement() => _color;

    public void LaunchGameContent(Vector2I position)
    {
        Position = position;

        _cordinatsInGrid = position / Game.GetSizePixel();
    }

    public void StartFall(Vector2I movePosition)
    {

    }

    public void DestroyElement() => Free();

    public void Move(Vector2 moveTarget)
    {               
        SetAnimation("position", moveTarget, _speed);
    }

    public void SelectElement()
    {
        Vector2 scale = Scale * 1.2f;

        SetAnimation("scale", scale, 0.1f);
    }

    public void StartDestroyAnimation()
    {
        Vector2 scale = Scale * 1.5f;

        SetAnimation("scale", scale, 0.25f);
    }
    
    public void FinishDestroyAnimation()
    {
        Vector2 scale = new Vector2(0f, 0f);

        SetAnimation("scale", scale, 0.25f);
    }

    public void UnselectElement()
    {
        Vector2 scale = Scale / 1.2f;
       
        SetAnimation("scale", scale, 0.1f);
    }

    public void PlaySpawnAnimation()
    {
        Vector2 scale = Scale;

        Scale = new Vector2(0f, 0f);

        SetAnimation("scale", scale, 0.05f);
    }

    private void SetAnimation(string nameProperty, Variant finalValue, float duration)
    {        
        _tweenMove = CreateTween();
        _tweenMove.BindNode(this);
        _tweenMove.SetTrans(Tween.TransitionType.Linear);
        _tweenMove.SetEase(Tween.EaseType.Out);
        _tweenMove.TweenProperty(this, nameProperty, finalValue, duration);
    }

    public float GetSpeed() => _speed;

    public int GetReward() => _reward;

    public TypeGameBoardContent GetTypeContent() => _typeContent;

    public Vector2I GetPosition() => new Vector2I((int)Position.X, (int)Position.Y);

    public Vector2I GetCordinatInGrid() => _cordinatsInGrid;
    public Vector2I SetCordinatInGrid(Vector2I newCord) => _cordinatsInGrid = newCord;
}

public interface IGameBoardContent
{
    void LaunchGameContent(Vector2I position);

    void StartFall(Vector2I movePosition);

    void DestroyElement();

    TypeGameBoardContent GetTypeContent();
}

public enum TypeBonusElement
{
    horizontalLine = 0,
    verticalLine = 1,
    bomb = 2
}

public enum TypeGameBoardContent
{
    emty = 0,
    redColor = 1,
    blueColor = 2,
    purpleColor = 3,
    beigeColor = 4,
    yellowColor = 5,    
}

public interface IReward
{
    int GetReward();
}