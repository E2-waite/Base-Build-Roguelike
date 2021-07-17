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

        Vector2 iconPos = startIcon, healthBarPos = startInfo;
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

            iconPos.y -= offset;
            healthBarPos.y -= offset;
            resizeVal = i * offset;
        }

        return resizeVal;
    }
}
