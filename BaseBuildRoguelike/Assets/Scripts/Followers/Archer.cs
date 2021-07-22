using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    public enum State
    {
        idle = 0,
        move = 1,
        attack = 2,
        defend = 3
    }
    public GuardTower guardTower;
    public float fireRange = 5f, shotTime = 0.5f, shotSpeed = 10;
    public const float cooldownTime = 2.5f;
    public GameObject arrowPrefab;
    public Cooldown shotCooldown = new Cooldown(cooldownTime);
    public override void BuildingDirect(Building building)
    {
        if (building is GuardTower && squad == null)
        {
            (building as GuardTower).AddArcher(this);
        }
    }
    private void Update()
    {
        shotCooldown.Tick();
        TickEffects();
        if (guardTower == null)
        {
            Swarm();
            if (state == (int)State.move)
            {
                if (transform.position == marker.transform.position)
                {
                    state = (int)State.idle;
                }
                else
                {
                    Move();
                }
            }
            else
            {
                if (target == null || target.interact == null)
                {
                    if (state == (int)State.attack)
                    {
                        if (Targetting.FindTarget(ref target, squad, transform.position, Enemies.enemies))
                        {
                            // Debug.Log("Target Found");
                        }
                        else
                        {
                            state = (int)State.move;
                        }
                    }
                    else
                    {
                        state = (int)State.idle;
                    }
                }
                else
                {
                    float dist = Vector2.Distance(transform.position, target.Position());
                    if (dist <= fireRange)
                    {
                        // Moves away from target if not charging up shot, the target is an enemy and this archer is too close
                        //if (!attacking && dist < fireRange - 3f && target is Enemy)
                        //{
                        //    Move(transform.position + ((transform.position - target.transform.position).normalized));
                        //}
                        if (state == (int)State.attack && shotCooldown.Complete() && interactRoutine == null)
                        {
                            interactRoutine = StartCoroutine(FireRoutine());
                        }
                    }
                    else
                    {
                        Move();
                    }
                }
            }
        }
        else
        {
            if (target.interact == null)
            {
                if (state == (int)State.defend)
                {
                    Interaction newTarget = Targetting.GetClosestTarget(guardTower.inRange, guardTower.transform.position);
                    if (newTarget == null)
                    {
                        state = (int)State.idle;
                    }
                    else
                    {
                        target = new Target(newTarget);
                    }
                }
            }
            else
            {
                if (state == (int)State.defend)
                {
                    float dist = Vector2.Distance(guardTower.transform.position, target.Position());
                    if (dist > guardTower.range)
                    {
                        target = new Target();
                    }
                    else if (shotCooldown.Complete() && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(FireRoutine());
                    }
                }
                else
                {
                    state = (int)State.defend;
                }
            }
        }
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(shotTime);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Setup(target.interact, this, shotSpeed, hitDamage);
        shotCooldown.Reset();
    }
}