using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConstructionInspector : InspectorDetails
{
    public Image[] icons;
    public Text[] texts;

    public override int Reload(Interaction selected)
    {
        Construct construct = (selected as Building).construct;

        if (icons != null)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                Destroy(icons[i]);
                Destroy(texts[i]);
            }
        }

        Vector2 iconPos = startIcon, textPos = startInfo;
        icons = new Image[Resources.NUM];
        texts = new Text[Resources.NUM];
        int resizeVal = 0;


        for (int i = 0; i < Resources.NUM; i++)
        {
            if (construct.cost[i] > 0)
            {
                GameObject icon = Instantiate(iconPrefab, transform);

                icon.GetComponent<RectTransform>().anchoredPosition = iconPos;
                icons[i] = icon.GetComponent<Image>();
                icons[i].sprite = Icons.Resource((Resource.Type)i);

                GameObject valueText = Instantiate(textPrefab, transform);
                valueText.GetComponent<RectTransform>().anchoredPosition = textPos;
                texts[i] = valueText.GetComponent<Text>();
                texts[i].text = (construct.cost[i] - construct.remaining[i]).ToString() + "/" + construct.cost[i].ToString();

                iconPos.y -= offset;
                textPos.y -= offset;
                if (i > 0)
                {
                    resizeVal += offset;
                }
            }
        }
        return resizeVal;
    }
}
