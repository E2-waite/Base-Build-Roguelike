using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DescriptionBox : MonoSingleton<DescriptionBox>
{
    public Text text;
    public GameObject box;
    public bool isEnabled = false;

    public void EnableBox(string name)
    {
        box.SetActive(true);
        text.text = name;
        isEnabled = true;
    }

    public void DisableBox()
    {
        box.SetActive(false);
        isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            box.transform.position = Input.mousePosition;
        }
    }
}
