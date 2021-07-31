using UnityEngine;
using System.Collections;

public class House : Building
{
    public int size = 5;
    public override void Setup()
    {
        Followers.AdjustMaxFollowers(size);
    }
}
