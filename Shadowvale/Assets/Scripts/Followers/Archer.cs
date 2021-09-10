using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Combat
{
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
    public override void Update()
    {
        shotCooldown.Tick();
        base.Update();
    }

    public override bool Attack()
    {
        float dist = Vector2.Distance(transform.position, currentAction.target.Position());
        if (dist <= fireRange)
        {
            if (shotCooldown.Complete() && interactRoutine == null)
            {
                interactRoutine = StartCoroutine(FireRoutine());
            }
            return true;
        }
        return false;
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(shotTime);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Setup(currentAction.target.interact, this, shotSpeed, hitDamage);
        shotCooldown.Reset();
    }
}