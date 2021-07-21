using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trainer : Building
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
    const int maxTraining = 3;
    protected List<Training> training = new List<Training>();

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

    public virtual void FinishTraining(int index) {}
}
