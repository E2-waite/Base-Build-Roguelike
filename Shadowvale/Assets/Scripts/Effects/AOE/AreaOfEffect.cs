using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    public enum Type
    {
        singleBurst,
        byTick
    }


    const int MAX_RADIUS = 2;

    public Type type = Type.byTick;

    public Sprite[] glowRadius = new Sprite[MAX_RADIUS];
    public int radius = 1, ticks = 4;
    public float duration = 3, fadeSpeed = 5;

    private List<Interaction> inRange = new List<Interaction>();
    private SpriteRenderer rend;
    private CircleCollider2D col;
    private Cooldown timer = null, tickTimer = null;
    private bool started = false;
    private float startAlpha = 0;
   
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
        rend.sprite = glowRadius[radius - 1];
        startAlpha = rend.color.a;
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0);
        col.radius = radius;
        timer = new Cooldown(duration);
        tickTimer = new Cooldown(duration / ticks);
        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        if (type == Type.byTick)
        {
            if (started && timer != null && tickTimer != null)
            {
                if (timer.Tick())
                {
                    Effect();
                    StartCoroutine(FadeOut());
                    // Destroy AOE effect after timer is complete
                }
                else
                {
                    // Do effect every tick 
                    Tick();
                }
            }
        }
    }

    bool Tick()
    {
        if (tickTimer.Tick())
        {
            Debug.Log("TICK");
            tickTimer.Reset();
            Effect();
            return true;
        }
        return false;
    }

    public virtual void Effect()
    {
        Debug.Log("EFFECT");
    }

    IEnumerator FadeIn()
    {
        while (rend.color.a < startAlpha)
        {
            Color newColour = rend.color;
            newColour.a += fadeSpeed * Time.deltaTime;
            rend.color = newColour;
            yield return null;
        }
        if (type == Type.byTick)
        {
            started = true;
        }
        else if (type == Type.singleBurst)
        {
            Effect();
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        started = false;
        while (rend.color.a > 0)
        {
            Color newColour = rend.color;
            newColour.a -= fadeSpeed * Time.deltaTime;
            rend.color = newColour;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inRange.Add(collision.GetComponent<Interaction>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inRange.Add(collision.GetComponent<Interaction>());
    }

}
