using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
public class MainMenu : MonoBehaviour, IPointerClickHandler
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
                if (eventData.pointerCurrentRaycast.gameObject == gameSaves[i])
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
    }
}
