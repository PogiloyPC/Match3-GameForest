using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class ExplosionParticles : GpuParticles2D
{
    [Export] private double _destroyTime = 0.3f;

    private Action<ExplosionParticles> _onReturn;

    private bool _isPlay;

    public void InitializeExplosion(Action<ExplosionParticles> onReturn) => _onReturn = onReturn;

    public void SetColorExplosion(Color color)
    {

    }

    public void SetPosition(Vector2 position) => Position = position;

    public override void _Process(double delta)
    {
        if (!_isPlay)
            return;

        _destroyTime -= delta;

        if (_destroyTime <= 0)
        {
            Stop();

            Free();

            //_onReturn.Invoke(this);
        }
    }

    public void Play()
    {
        if (_isPlay)
            Stop();

        ChangePlayback(true, true);
    }

    public void Stop()
    {
        ChangePlayback(false, false);       
    }

    private void ChangePlayback(bool emitting, bool isPlay)
    {
        Emitting = emitting;
        
        _isPlay = isPlay;
    }
}

