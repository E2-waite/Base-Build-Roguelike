using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : Follower
{
    public enum CombatState
    {
        idle = 0,
        move = 1,
        attack = 2,
        defend = 3
    }

    public override void Update()
    {
        base.Update();

        if (currentAction.state == (int)Combat.CombatState.move)
        {
            if (path.Count == 0)
            {
                Idle();
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (currentAction.target.interact == null)
            {
                if (currentAction.state == (int)CombatState.attack)
                {
                    if (!GetDetectedTarget())
                    {
                        MoveTo(marker.transform.position);
                    }
                }
                else
                {
                    Idle();
                }
            }
            else
            {
                if (currentAction.state != (int)CombatState.attack || !Attack())
                {
                    Move();
                }
            }
        }

    }

    public virtual bool Attack()
    {
        return true;
    }

    public override void Direct(Vector2 pos, Interaction obj)
    {
        base.Direct(pos, obj);
        int state = 0;
        Target target = new Target();
        if (obj != null)
        {
            target = new Target(obj);

            marker.transform.position = obj.transform.position;

            if (target.interact is Enemy)
            {
                state = (int)CombatState.attack;
            }
            else if (target.interact is Follower)
            {
                Follower follower = target.interact as Follower;
                if (follower is Combat || follower is Support)
                {
                    JoinSquad(follower);
                }
            }
        }
        else
        {
            target = new Target();
            state = (int)CombatState.move;
        }

        // Adds the action to the list of actions and sets as current target
        actions.Add(new Action(target, state));
        currentAction = actions[actions.Count - 1];
    }
}
