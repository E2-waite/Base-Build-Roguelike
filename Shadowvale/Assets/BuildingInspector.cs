using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildingInspector : InspectorDetails
{
    public InspectorDetails currentDetails;
    public InspectorDetails constructionDetails, trainerDetails, guardDetails, storageDetails, homeDetails;
    public Image healthBar;

    public override int Reload(Interaction selected)
    {
        Building building = selected as Building;
        healthBar.fillAmount = (float)building.repair / (float)building.maxRepair;
        if (building.isConstructed)
        {
            if (building is Trainer)
            {
                SwapDetails(trainerDetails);
            }    
            else if (building is GuardTower)
            {
                SwapDetails(guardDetails);
            }
            else if (building is ResourceStorage)
            {
                SwapDetails(storageDetails);
            }
            else if (building is HomeBase)
            {
                SwapDetails(homeDetails);
            }
            else
            {
                if (currentDetails != null)
                {
                    currentDetails.gameObject.SetActive(false);
                    currentDetails = null;
                }
            }
        }
        else
        {
            SwapDetails(constructionDetails);
        }

        if (currentDetails == null)
        {
            return 0;
        }
        else
        {
            return currentDetails.Reload(selected) + 50;
        }
    }

    void SwapDetails(InspectorDetails newDetails)
    {
        if (currentDetails != newDetails)
        {
            // Swap details menu
            if (currentDetails != null)
            {
                currentDetails.gameObject.SetActive(false);
            }
            newDetails.gameObject.SetActive(true);
        }
        currentDetails = newDetails;
    }
}
