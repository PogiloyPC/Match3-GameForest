using Godot;
using System;

public partial class TimerSwap : Timer
{
    public Action<ElementGameBoard, ElementGameBoard> TimeOut;
    
    public bool IsRunning { get; private set; }    
    
    private ElementGameBoard _firstElement;
    private ElementGameBoard _secondElement;

    private double _waitTime;

    public void InitializeTimeout(ElementGameBoard firstElement, ElementGameBoard secondElement)
    {
        _firstElement = firstElement;
        _secondElement = secondElement;
    }

    public void SetTimeTimer(float time) => _waitTime = time;

    public override void _Process(double delta)
    {
        if (!IsRunning)
            return;

        _waitTime -= delta;

        if (_waitTime <= 0)
        {
            TimeOut?.Invoke(_firstElement, _secondElement);

            IsRunning = false;

            _firstElement = null;
            _secondElement = null;
        }
    }

    public void SetPlay() => IsRunning = true;
    public void SetPause() => IsRunning = false;
}

