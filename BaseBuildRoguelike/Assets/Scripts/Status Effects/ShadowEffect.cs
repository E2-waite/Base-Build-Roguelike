using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEffect : StatusEffect
{
    public ShadowEffect(float duration = 1, float interval = 1, Interaction target = null)
    {
        effect = target.StartCoroutine(Effect(duration, interval, target));
    }

    private IEnumerator Effect(float duration, float interval, Interaction target)
    {
        float timer = 0;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                timer = 0;
                if (target is Follower)
                {
                    (target as Follower).Hit(1, null);
                }
            }
            yield return null;
        }
        target.RemoveEffect(this);
    }
}
