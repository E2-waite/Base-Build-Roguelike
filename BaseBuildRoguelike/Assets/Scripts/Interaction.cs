using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public bool staticObject = false;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public void AddEffect(StatusEffect effect)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (effect.GetType() == statusEffects[i].GetType())
            {
                statusEffects[i].StopEffect(this);
                statusEffects[i] = effect;
                return;
            }
        }
        statusEffects.Add(effect);
    }

    public void RemoveEffect(StatusEffect effect)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (effect == statusEffects[i])
            {
                statusEffects.RemoveAt(i);
            }
        }
    }
}
 