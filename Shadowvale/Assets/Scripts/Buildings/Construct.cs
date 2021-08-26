using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Sprite contruction, constructed;
    #if UNITY_EDITOR
    [EnumNamedArray(typeof(Resource.Type))]
    #endif
    public int[] cost = new int[Resources.NUM], remaining = new int[Resources.NUM];
    Building building;
    SpriteRenderer rend;
    Animator anim;
    public RuntimeAnimatorController buildingAnimator = null;
    private void Start()
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            remaining[i] = cost[i];
        }
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rend.sprite = contruction;
        building = GetComponent<Building>();
        CheckComplete();
    }

    public bool Build()
    {
        if (!building.isConstructed)
        {
            bool success = false;
            for (int i = 0; i < Resources.NUM; i++)
            {
                if (remaining[i] > 0 && Resources.resources[i] > 0 && Buildings.UseResource((Resource.Type)i, 1))
                {
                    remaining[i]--;
                    success = true;
                }
            }

            if (success)
            {
                StartCoroutine(HitRoutine());
                building.ReloadInspector();
                CheckComplete();
                return true;
            }
        }
        return false;
    }

    IEnumerator HitRoutine()
    {
        anim.SetBool("Hit", true);

        yield return new WaitForSeconds(0.1f);
        if (anim != null)
        {
            anim.SetBool("Hit", false);
        }
    }

    public void CheckComplete()
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            if (remaining[i] > 0)
            {
                return;
            }
        }

        rend.sprite = constructed;
        building.Constructed();
        if (buildingAnimator != null)
        {
            anim.runtimeAnimatorController = buildingAnimator;
        }
        else
        {
            Destroy(anim);
        }
        Destroy(this);
    }

    public bool Complete(BuildingData data)
    {
        if (data.constructed)
        {
            rend = GetComponent<SpriteRenderer>();
            building = GetComponent<Building>();
            rend.sprite = constructed;
            building.Constructed();
            anim = GetComponent<Animator>();
            if (buildingAnimator != null)
            {
                anim.runtimeAnimatorController = buildingAnimator;
            }
            else
            {
                Destroy(anim);
            }
            Destroy(this);
            return true;
        }
        return false;
    }
}
