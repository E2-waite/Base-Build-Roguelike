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
        Swarm();
        if (currentAction.state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
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
                if (currentAction.state == (int)State.attack)
                {
                    //if (Targetting.FindTarget(ref target, squad, transform.position, Enemies.enemies))
                    //{
                    //    // Debug.Log("Target Found");
                    //}
                    //else
                    //{
                    //    state = (int)State.move;
                    //}

                    // Currently just goes back to move state rather than finding a new target
                    MoveTo(marker.transform.position);
                }
                else
                {
                    Idle();
                }
            }
            else
            {
                float dist = Vector2.Distance(transform.position, currentAction.target.Position());
                if (dist <= fireRange)
                {
                    // Moves away from target if not charging up shot, the target is an enemy and this archer is too close
                    //if (!attacking && dist < fireRange - 3f && target is Enemy)
                    //{
                    //    Move(transform.position + ((transform.position - target.transform.position).normalized));
                    //}
                    if (currentAction.state == (int)State.attack && shotCooldown.Complete() && interactRoutine == null)
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

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(shotTime);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Setup(currentAction.target.interact, this, shotSpeed, hitDamage);
        shotCooldown.Reset();
    }
}