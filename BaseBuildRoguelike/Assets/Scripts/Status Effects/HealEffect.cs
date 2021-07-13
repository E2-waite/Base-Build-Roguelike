using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : StatusEffect
{
    public HealEffect(float duration = 1, float interval = 1, Interaction target = null)
    {
        effect = target.StartCoroutine(Effect(duration, interval, target));
    }

    private IEnumerator Effect(float duration, float interval, Interaction target)
    {
        float timer = 0;
        (target as Follower).Heal(1);
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0;
                if (target is Follower)
                {
                    (target as Follower).Heal(1);
                }
            }
            yield return null;
        }
        target.RemoveEffect(this);
    }
}
