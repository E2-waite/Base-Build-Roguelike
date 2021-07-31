using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoSingleton<HUD>
{
    public Text[] resourceText = new Text[3];
    public Text followersVal;

    public void UpdateResources(int[] resources, int[] maxResources)
    {
        for (int i = 0; i < 3; i++)
        {
            resourceText[i].text = resources[i].ToString() + "/" + maxResources[i].ToString();
        }
    }

    public void UpdateFollowers()
    {
        followersVal.text = Followers.followers.Count.ToString() + "/" + Followers.maxFollowers.ToString();
    }
}
