using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public int index = 0;
    public int ticks, value;
    public float duration, tickTimer;
    public readonly float tickTime;
    [System.NonSerialized] public Interaction target;

    public StatusEffect(Interaction _target, int _ticks, float _duration, int _value)
    {
        target = _target;
        ticks = _ticks;
        duration = _duration;
        tickTime = duration / ticks;
        value = _value;
    }

    public bool Tick()
    {
        if (ticks <= 0)
        {
            return false;
        }
        if (tickTimer <= 0)
        {
            ticks--;
            tickTimer = duration / ticks;
            Effect();
        }
        tickTimer -= Time.deltaTime;
        return true;
    }

    public float TimeLeft()
    {
        return (ticks * tickTime) + tickTimer;
    }

    protected virtual void Effect()
    {

    }
}

[System.Serializable]
public class ShadowEffect : StatusEffect
{
    public ShadowEffect(Interaction _target, int _ticks, float _duration, int _value) : base(_target, _ticks, _duration, _value) { }
    protected override void Effect()
    {
        if (target is Follower)
        {
            (target as Follower).Hit(value, null);
        }
    }
}

[System.Serializable]
public class HealEffect : StatusEffect
{
    public HealEffect(Interaction _target, int _ticks, float _duration, int _value) : base(_target, _ticks, _duration, _value) { }
    protected override void Effect()
    {
        if (target is Follower)
        {
            (target as Follower).Heal(value);
        }
    }
}