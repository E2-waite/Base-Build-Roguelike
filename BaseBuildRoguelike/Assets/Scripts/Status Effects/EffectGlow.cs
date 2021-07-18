using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGlow : MonoBehaviour
{
    SpriteRenderer rend;
    public Color healColour, shadowColour;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetupGlow(List<StatusEffect> effects)
    {
        rend = GetComponent<SpriteRenderer>();
        if (effects.Count == 0)
        {
            rend.enabled = false;
        }
        else
        {
            rend.enabled = true;
            Color colour = Color.white;
            for (int i = 0; i < effects.Count; i++)
            {
                colour = NewColour(effects[i]);
            }
            rend.color = colour;
        }
    }

    public void UpdateGlow(List<StatusEffect> effects)
    {
        if (effects.Count == 0)
        {
            rend.enabled = false;
        }
        else
        {
            rend.enabled = true;
            Color colour = Color.white;
            for (int i = 0; i < effects.Count; i++)
            {
                colour = NewColour(effects[i]);
            }
            rend.color = colour;
        }
    }

    Color NewColour(StatusEffect effect)
    {
        if (effect is ShadowEffect)
        {
            return shadowColour;
        }
        else if (effect is HealEffect)
        {
            return healColour;
        }
        return Color.white;
    }
}
