using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryRange : Building
{
    public struct Training
    {
        public Follower follower;
        public Cooldown time;

        public Training(Follower _follower, Cooldown _time)
        {
            follower = _follower;
            time = _time;
        }
    }
    public GameObject archerPrefab;
    const int maxTraining = 3;
    List<Training> training = new List<Training>();

    private void Update()
    {
        for (int i = 0; i < training.Count; i++)
        {
            if (training[i].time.Tick())
            {
                FinishTraining(i);
            }
        }
    }

    public bool AddFollower(Follower follower)
    {
        if (training.Count < maxTraining)
        {
            training.Add(new Training(follower, new Cooldown(5)));
            return true;
        }
        return false;
    }

    void FinishTraining(int index)
    {
        GameObject archer = Instantiate(archerPrefab, training[index].follower.transform.position, Quaternion.identity);
        Followers.Remove(training[index].follower);
        Destroy(training[index].follower.gameObject);
        Followers.Add(archer.GetComponent<Archer>());
        training.RemoveAt(index);
    }
}
