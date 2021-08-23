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

    public override bool Save(BuildingData data)
    {
        if (!base.Save(data))
        {
            return false;
        }
        data.members = new int[training.Length];
        data.timers = new Cooldown[training.Length];
        for (int i = 0; i < training.Length; i++)
        {
            if (training[i] != null && training[i].follower != null)
            {
                data.members[i] = training[i].follower.Index();
                data.timers[i] = training[i].time;
            }
            else
            {
                data.members[i] = -1;
                data.timers[i] = null;
            }
        }
        return true;
    }

    public override void LoadInstance()
    {
        for (int i = 0; i < 3; i++)
        {
            if (buildingData.members[i] >= 0)
            {
                Debug.Log(buildingData.members[i]);
                AddFollower(Grid.TargetFromIndex(buildingData.members[i]) as Follower, i, buildingData.timers[i]);
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
    public override void Destroy()
    {
        for (int i = 0; i < 3; i++)
        {
            if (training != null && training[i] != null)
            {
                training[i].follower.gameObject.SetActive(true);
            }
        }
        base.Destroy();
    }

    public virtual void FinishTraining(int index) {}
}
