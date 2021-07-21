using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Trainer
{
    public GameObject soldierPrefab;

    public override void FinishTraining(int index)
    {
        GameObject soldier = Instantiate(soldierPrefab, training[index].follower.transform.position, Quaternion.identity);
        Followers.Remove(training[index].follower);
        Destroy(training[index].follower.gameObject);
        Followers.Add(soldier.GetComponent<Soldier>());
        training[index] = null;
    }
}
