using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : Follower
{
    public enum SupportState
    {
        idle = 0,
        move = 1,
        support = 2,
    }

    Cooldown searchCooldown = new Cooldown(1);
    public float supportRange = 5;
    public override void Update()
    {
        base.Update();

        searchCooldown.Tick();
        //if (state == (int)State.move)
        //{
        //    if (transform.position == marker.transform.position)
        //    {
        //        state = (int)State.idle;
        //    }
        //    else
        //    {
        //        Move();
        //    }
        //}

        if (currentAction.state == (int)SupportState.idle && searchCooldown.Complete())
        {
            searchCooldown.Reset();
            if (FindTarget())
            {
                Debug.Log("Found Support Target");
            }
        }

        if (currentAction.state == (int)SupportState.move)
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
            if (currentAction.state == (int)SupportState.support)
            {
                float dist = Vector2.Distance(transform.position, currentAction.target.Position());
                if (dist > supportRange)
                {
                    Move();
                }
                else if (SupportFollower())
                {
                    Idle();
                }
            }
        }
    }

    public virtual bool SupportFollower()
    {
        return true;
    }

    public virtual bool FindTarget()
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

            if (target.interact is Enemy && squad != null)
            {
                state = (int)SupportState.support;
                // Need to target follower
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
            state = (int)SupportState.move;
        }

        // Adds the action to the list of actions and sets as current target
        actions.Add(new Action(target, state));
        currentAction = actions[actions.Count - 1];
    }
}
