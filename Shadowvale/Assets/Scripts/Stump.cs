using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump : Interaction
{
    public Resource.Size size;
    public float decayTime = 60;

    private void Start()
    {
        StartCoroutine(KillDelay());
    }

    IEnumerator KillDelay()
    {
        yield return new WaitForSeconds(decayTime);
        Destroy(gameObject);
    }
}
