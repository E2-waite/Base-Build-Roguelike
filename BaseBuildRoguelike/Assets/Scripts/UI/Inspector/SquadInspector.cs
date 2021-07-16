using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SquadInspector : InspectorDetails
{
    public Image[] icons, healthBars;

    public override int Reload(Interaction selected)
    {
        Squad squad = (selected as Follower).squad;

        if (icons != null)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                Destroy(icons[i]);
                Destroy(healthBars[i]);
            }
        }

        Vector2 iconPos = new Vector2(-195, 42), healthBarPos = new Vector2(-165, 33);
        icons = new Image[squad.members.Count];
        healthBars = new Image[squad.members.Count];
        int resizeVal = 0;

        for (int i = 0; i < squad.members.Count; i++)
        {
            Follower follower = squad.members[i] as Follower;
            GameObject icon = Instantiate(iconPrefab, transform);

            icon.GetComponent<RectTransform>().anchoredPosition = iconPos;
            icons[i] = icon.GetComponent<Image>();
            icons[i].sprite = Icons.Follower(follower.type);

            GameObject healthBar = Instantiate(healthBarPrefab, transform);
            healthBar.GetComponent<RectTransform>().anchoredPosition = healthBarPos;
            healthBars[i] = healthBar.GetComponent<Image>();
            healthBars[i].fillAmount = (float)follower.health / (float)follower.maxHealth;

            iconPos.y -= 35;
            healthBarPos.y -= 35;
            resizeVal = i * 35;
        }

        return resizeVal;
    }
}
