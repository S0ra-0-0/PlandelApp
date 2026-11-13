using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public bool settingsState;
    [SerializeField] private List<GameObject> SettingsUI;

    private void Awake()
    {
        if(instance == null && instance != this)
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
            for (int i = 0; i < SettingsUI.Count; i++)
            {
                SettingsUI[i].SetActive(true);
            }
            settingsState = true;
        }
        else if (settingsState)
        {
            for (int i = 0; i < SettingsUI.Count; i++)
            {
                SettingsUI[i].SetActive(false);
            }
            settingsState = false;
        }
    }
}
