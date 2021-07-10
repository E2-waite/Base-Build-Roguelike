using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Targetting
{
    static float targetRange = 15;
    static Interaction GetClosestTarget(List<Interaction> targets, Vector3 pos)
    {
        Interaction newTarget = null;
        float closestDist = 9999;

        foreach (Interaction target in targets)
        {
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

    public static bool FindTarget(ref Interaction target, Squad squad, Squad targetSquad, Vector3 pos, List<Interaction> targets)
    {
        if (squad == null)
        {
            if (targetSquad == null)
            {
                target = Targetting.GetClosestTarget(targets, pos);
                if (target != null)
                {
                    return true;
                }
            }
            else
            {
                // Targets closest enemy in targetted squad
                target = targetSquad.ClosestMember(pos);
                return true;
            }
        }
        else
        {
            if (squad.target != null)
            {
                // Targets squad's current target
                target = squad.target;
                return true;
            }
            else if (squad.targetSquad != null)
            {
                // Finds closest enemy in squad's targetted enemy squad
                target = squad.targetSquad.ClosestMember(pos);
                return true;
            }
            else
            {
                // Finds non-targetted enemy in range, then sets the squad target to that enemy
                target = Targetting.GetClosestTarget(targets, pos);
                if (target != null)
                {
                    squad.SetTarget(target);
                    return true;
                }
            }
        }
        return false;
    }
}
