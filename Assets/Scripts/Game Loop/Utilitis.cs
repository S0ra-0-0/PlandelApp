using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilitis : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchScenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PressSettings()
    {
        GameManager.instance.Settings();
    }

    public void PressTutorial()
    {
        GameManager.instance.Tutorial();
    }

    public void PressNotifications()
    {
        GameManager.instance.NotificationSettings();
    }

    public void NotificationsOn()
    {
        GameManager.instance.NotificationStatusOn();
    }

    public void NotificationsOff()
    {
        GameManager.instance.NotificationStatusOff();
    }
}
