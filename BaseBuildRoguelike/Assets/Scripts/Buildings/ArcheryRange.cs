using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryRange : Trainer
{

    public GameObject archerPrefab;

    public override void FinishTraining(int index)
    {
        GameObject archer = Instantiate(archerPrefab, training[index].follower.transform.position, Quaternion.identity);
        Followers.Remove(training[index].follower);
        Destroy(training[index].follower.gameObject);
        Followers.Add(archer.GetComponent<Archer>());
        training.RemoveAt(index);
    }
}
