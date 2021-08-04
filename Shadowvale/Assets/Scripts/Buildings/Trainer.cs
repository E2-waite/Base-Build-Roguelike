using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trainer : Building
{
    public class Training
    {
        public Follower follower;
        public Cooldown time;

        public Training(Follower _follower, Cooldown _time)
        {
            follower = _follower;
            time = _time;
        }
    }
    public Training[] training = new Training[3];


    public override void Load(BuildingData data)
    {
        for (int i = 0; i < 3; i++)
        {
            if (data.members[i] >= 0)
            {
                Debug.Log(data.members[i]);
                AddFollower(Grid.TargetFromIndex(data.members[i]) as Follower, i, data.timers[i]);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (training[i] != null && training[i].time.Tick())
            {
                FinishTraining(i);
            }
        }
    }

    public bool AddFollower(Follower follower, int index, Cooldown time = null)
    {
        if (training[index] == null)
        {
            training[index] = new Training(follower, (time == null) ? new Cooldown(5) : time);
            Followers.selected = null;
            follower.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
    public override void DestroyThis()
    {
        for (int i = 0; i < 3; i++)
        {
            if (training != null && training[i] != null)
            {
                training[i].follower.gameObject.SetActive(true);
            }
        }
    }

    public virtual void FinishTraining(int index) {}
}
