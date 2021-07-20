using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Targetting
{
    public static float targetRange = 15;
    static Interaction GetClosestTarget<T>(List<T> targets, Vector3 pos)
    {
        // Gets closest target within range out of all target of type
        Interaction newTarget = null;
        float closestDist = 9999;

        foreach (T tempTarget in targets)
        {
            Interaction target = tempTarget as Interaction;
            if (target != null)
            {
                float dist = Vector3.Distance(pos, target.transform.position);

                if (dist <= targetRange && dist < closestDist)
                {
                    closestDist = dist;
                    newTarget = target;
                }
            }
        }

        return newTarget;
    }

    class TargetList<T>
    {
        public T List { get; set; }
    }

    public static bool FindTarget<T>(ref Target target, Squad squad, Vector3 pos, List<T> targets)
    {
        if (squad == null)
        {
            if (target.squad == null)
            {
                target = new Target(Targetting.GetClosestTarget(targets, pos));
                Debug.Log("Got closest target");
                if (target.interact != null)
                {
                    Debug.Log("RETURNING FUCKING TRUE");
                    return true;
                }
            }
            else
            {
                // Targets closest enemy in targetted squad
                target = new Target(target.squad.ClosestMember(pos));

                return true;
            }
        }
        else
        {
            if (squad.target != null)
            {
                if (squad.target.squad == null)
                {
                    // Targets squad's current target
                    target = new Target(squad.target.interact);
                }
                else
                {
                    // Finds closest enemy in squad's targetted enemy squad
                    target = new Target(squad.target.squad.ClosestMember(pos));
                }
                return true;
            }
            else
            {
                // Finds non-targetted enemy in range, then sets the squad target to that enemy
                target = new Target(GetClosestTarget(targets, pos));
                if (target.interact != null)
                {
                    squad.SetTarget(target.interact);
                    return true;
                }
            }
        }
        return false;
    }
}
