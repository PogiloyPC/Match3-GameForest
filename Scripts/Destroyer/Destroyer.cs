using Godot;

public partial class Destroyer : Node2D
{
    [Export] private AnimatedSprite2D _anim;

    private Tween _tween;
  
    public void MoveNextPosition(Vector2 target, bool isRight)
    {
        if (isRight)
            _anim.FlipH = true;
        else
            _anim.FlipH = false;

        _anim.Play();

        _tween = CreateTween();
        _tween.BindNode(this);
        _tween.SetTrans(Tween.TransitionType.Linear);
        _tween.SetEase(Tween.EaseType.Out);
        _tween.TweenProperty(this, "position", target, 0.1f);
    }
}

