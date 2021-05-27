using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public List<Sprite> stages = new List<Sprite>();

    public Resource.Type type;
    public int maxStorage = 100, currentStorage = 0;

    SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = stages[0];
    }


    private void Update()
    {
        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
    }

    public void Store(ref int val)
    {
        if (currentStorage + val > maxStorage)
        {
            int diff = maxStorage - currentStorage;

            currentStorage += diff;
            val -= diff;
        }
        else
        {
            currentStorage += val;
            val = 0;
        }
    }
}
