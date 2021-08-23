using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public string inspectorName = "";
    public bool staticObject = false;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    public EffectGlow glow;
    public void AddEffect(StatusEffect newEffect)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (newEffect.GetType() == statusEffects[i].GetType())
            {
                statusEffects[i] = newEffect;
                return;
            }
        }
        statusEffects.Add(newEffect);
        if (glow != null)
        {
            glow.UpdateGlow(statusEffects);
        }
    }

    public void TickEffects()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            // Update the status effect - if completed (returned false) remove the effect
            if (!statusEffects[i].Tick())
            {
                statusEffects.RemoveAt(i);
                glow.UpdateGlow(statusEffects);
            }
        }
    }

    public int Index()
    {
        List<Interaction> interactions = Grid.GetAllInteractable();
        for (int i = 0; i < interactions.Count; i++)
        {
            if (interactions[i] == this)
            {
                return i;
            }
        }
        return 0;
    }
}
 