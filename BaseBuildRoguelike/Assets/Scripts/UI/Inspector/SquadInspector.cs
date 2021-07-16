using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SquadInspector : InspectorDetails
{
    public Image[] icons;

    public override int Reload(Interaction selected)
    {
        Squad squad = (selected as Follower).squad;
        
        Vector2 pos = new Vector2(-210, 65);

        if (icons != null)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                Destroy(icons[i]);
            }
        }

        icons = new Image[squad.members.Count];
        int resizeVal = 0;

        for (int i = 0; i < squad.members.Count; i++)
        {
            GameObject icon = Instantiate(iconPrefab, transform);

            icon.GetComponent<RectTransform>().anchoredPosition = pos;
            icons[i] = icon.GetComponent<Image>();
            icons[i].sprite = Icons.Follower((squad.members[i] as Follower).type);
            pos.y -= 40;
            resizeVal = i * 40;
        }

        return resizeVal;
    }
}
