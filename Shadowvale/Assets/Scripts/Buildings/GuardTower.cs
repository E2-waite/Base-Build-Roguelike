using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTower : Building
{
    public float range = 5;
    public Archer archer;
    public List<Enemy> inRange = new List<Enemy>();
    public Enemy target;
    CircleCollider2D detectCol;
    public SpriteRenderer archerRend;
    public Animator archerAnim;
    public Cooldown shotCooldown = new Cooldown(2.5f);
    // Start is called before the first frame update
    public override void Setup()
    {
        detectCol = transform.GetChild(0).GetComponent<CircleCollider2D>();
        archerRend = transform.GetChild(1).GetComponent<SpriteRenderer>();
        archerAnim = transform.GetChild(1).GetComponent<Animator>();
    }

    public override void Load(BuildingData data)
    {
        if (isConstructed && data.members[0] >= 0)
        {
            AddArcher(Grid.TargetFromIndex(data.members[0]) as Archer);
            shotCooldown = data.timers[0];
        }
    }

    public void AddArcher(Archer newArcher)
    {
        if (archer == null)
        {
            archer = newArcher;
            newArcher.gameObject.SetActive(false);
            archerRend.gameObject.SetActive(true);
            archer.guardTower = this;
        }
    }

    public void RemoveArcher()
    {
        if (archer != null)
        {
            archerRend.gameObject.SetActive(false);
            archer.gameObject.SetActive(true);
            archer.transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
            archer.guardTower = null;
            archer = null;
        }
    }

    public override void DestroyThis()
    {
        RemoveArcher();
    }

    // Update is called once per frame
    void Update()
    {
        if (detectCol != null && detectCol.radius != range)
        {
            detectCol.radius = range;
        }

        if (archer != null)
        {
            shotCooldown.Tick();
            if (target == null)
            {
                if (inRange.Count > 0)
                {
                    target = Targetting.GetClosestTarget(inRange, transform.position) as Enemy;
                }
            }
            else
            {
                float diff = target.transform.position.y - transform.position.y;
                archerAnim.SetInteger("Direction", Mathf.RoundToInt(diff));
                if (shotCooldown.Complete())
                {
                    StartCoroutine(FireRoutine());
                    shotCooldown.Reset();
                }
            }
        }
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(archer.shotTime);
        GameObject arrow = Instantiate(archer.arrowPrefab, archerRend.transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Setup(target, this, archer.shotSpeed, archer.hitDamage);
    }
}
