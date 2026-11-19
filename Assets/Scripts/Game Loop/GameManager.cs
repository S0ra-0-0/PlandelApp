using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIObjects")]
    [SerializeField] private List<GameObject> settingsUI;

    [SerializeField] private List<GameObject> tutorialUI;

    [SerializeField] private List<GameObject> notificationUI;

    [Header("Setting Stats")]
    private bool settingsState;

    private bool tutorialState;

    private bool Notifications;



    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Settings()
    {
        if (!settingsState)
        {
            for (int i = 0; i < settingsUI.Count; i++)
            {
                settingsUI[i].SetActive(true);
            }
            settingsState = true;

            Debug.Log("test");
        }
        else if (settingsState)
        {
            for (int i = 0; i < settingsUI.Count; i++)
            {
                settingsUI[i].SetActive(false);
            }
            settingsState = false;
        }
    }

    //public void Tutorial()
    //{
    //    if (!tutorialState)
    //    {
    //        for (int i = 0; i < tutorialUI.Count; i++)
    //        {
    //            tutorialUI[i].SetActive(true);
    //        }
    //        tutorialState = true;
    //    }
    //    else if (tutorialState)
    //    {
    //        for (int i = 0; i < tutorialUI.Count; i++)
    //        {
    //            tutorialUI[i].SetActive(false);
    //        }
    //        tutorialState = false;
    //    }
    //}

    public void Tutorial()
    {
         for (int i = 0; i < settingsUI.Count; i++)
         {
            settingsUI[i].SetActive(false);
         }

         settingsState = false;

        for (int i = 0;i < tutorialUI.Count; i++)
        {
            tutorialUI[i].SetActive(true);
        }
    }

    public void NotificationSettings()
    {
        for (int i = 0; i < settingsUI.Count; i++)
        {
            notificationUI[i].SetActive(true);
        }
    }

    public void NotificationStatusOn()
    {
         Notifications = true;

        for (int i = 0; i < settingsUI.Count; i++)
        {
            notificationUI[i].SetActive(false);
        }
    }

    public void NotificationStatusOff()
    {
        Notifications = false;

        for (int i = 0; i < settingsUI.Count; i++)
        {
            notificationUI[i].SetActive(false);
        }
    }
}
