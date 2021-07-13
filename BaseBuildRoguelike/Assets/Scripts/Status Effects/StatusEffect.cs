using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    protected Coroutine effect;

    public void StopEffect(Interaction target)
    {
        target.StopCoroutine(effect);
    }
}
