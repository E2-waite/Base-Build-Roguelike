using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SquadInspector : InspectorDetails
{
    public Image[] icons;

    public override void Reload(Interaction selected)
    {
        Squad squad = (selected as Follower).squad;
        
        Vector2 pos = new Vector2(Screen.width - 25, Screen.height / 2);

        if (icons != null)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                Destroy(icons[i]);
            }
        }

        icons = new Image[squad.members.Count];

        for (int i = 0; i < squad.members.Count; i++)
        {
            GameObject icon = Instantiate(iconPrefab, transform);
            icon.transform.position = pos;
            icons[i] = icon.GetComponent<Image>();
            icons[i].sprite = Icons.Follower((squad.members[i] as Follower).type);
            pos.y -= 25;
        }
    }
}
