using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple : Trainer
{
    public GameObject priestPrefab;

    public override void FinishTraining(int index)
    {
        GameObject priest = Instantiate(priestPrefab, training[index].follower.transform.position, Quaternion.identity);
        Followers.Remove(training[index].follower);
        Destroy(training[index].follower.gameObject);
        Followers.Add(priest.GetComponent<Priest>());
        training[index] = null;
    }
}
