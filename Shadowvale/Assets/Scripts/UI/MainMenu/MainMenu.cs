using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum Buttons
    {
        play,
        settings,
        exit
    }

    public enum Menu
    {
        main,
        saves,
        settings
    }

    public Menu currentMenu = Menu.main;
    public GameObject[] menus;
    public GameObject[] menuButtons = new GameObject[3];
    public GameObject[] gameSaves = new GameObject[3];
    public Color textColour, rolloverColour;
    public GameObject backButton;
    private void Start()
    {
        for (int i = 0; i < gameSaves.Length; i++)
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/SaveData" + (i + 1).ToString() + ".json"))
            {
                gameSaves[i].transform.GetChild(0).GetComponent<Text>().text = ("-Save " + (i + 1).ToString() + "-");
            }
        }
        menus[1].SetActive(false);
        menus[2].SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMenu == Menu.main)
            {
                Application.Quit();
            }
            else
            {
                SwitchMenu(Menu.main);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == backButton)
        {
            SwitchMenu(Menu.main);
            return;
        }   
        
        if (currentMenu == Menu.main)
        {
            if (eventData.pointerCurrentRaycast.gameObject == menuButtons[(int)Buttons.play])
            {
                SwitchMenu(Menu.saves);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == menuButtons[(int)Buttons.settings])
            {
                SwitchMenu(Menu.settings);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == menuButtons[(int)Buttons.exit])
            {
                Application.Quit();
            }
        }
        else if (currentMenu == Menu.saves)
        {
            for (int i = 0; i < gameSaves.Length; i++)
            {
                if (eventData.pointerCurrentRaycast.gameObject == gameSaves[i].transform.GetChild(1))
                {
                    File.Delete(Application.persistentDataPath + "/SaveData" + (i + 1).ToString() + ".json");
                    gameSaves[i].transform.GetChild(0).GetComponent<Text>().text = "-Blank Save";
                    gameSaves[i].transform.GetChild(1).gameObject.SetActive(false);

                }
                else if (eventData.pointerCurrentRaycast.gameObject == gameSaves[i])
                {
                    Save.file = "SaveData" + (i + 1).ToString();
                    SceneManager.LoadScene(1);
                }
            }
        }

    }

    void SwitchMenu(Menu menu)
    {
        menus[(int)currentMenu].SetActive(false);
        currentMenu = menu;
        menus[(int)currentMenu].SetActive(true);

        if (menu == Menu.main)
        {
            backButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.pointerCurrentRaycast.gameObject == backButton)
        {
            backButton.transform.GetChild(0).GetComponent<Image>().color = rolloverColour;
        }
        if (pointerEventData.pointerCurrentRaycast.gameObject != null && pointerEventData.pointerCurrentRaycast.gameObject.CompareTag("Button"))
        {
            pointerEventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).GetComponent<Text>().color = rolloverColour;
        }
        if (currentMenu == Menu.saves)
        {
            for (int i = 0; i < 3; i++)
            {
                if (pointerEventData.pointerCurrentRaycast.gameObject == gameSaves[i] && System.IO.File.Exists(Application.persistentDataPath + "/SaveData" + (i + 1).ToString() + ".json"))
                {
                    gameSaves[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        backButton.transform.GetChild(0).GetComponent<Image>().color = textColour;
        for (int i = 0; i < 3; i++)
        {
            menuButtons[i].transform.GetChild(0).GetComponent<Text>().color = textColour;
            gameSaves[i].transform.GetChild(0).GetComponent<Text>().color = textColour;
            gameSaves[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
