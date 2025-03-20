using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Allows script to be directly referenced without variables and saves it across scenes
    #region Singleton

    private static SettingsManager instance;
    public static SettingsManager Instance
    {
        get => instance;
        private set
        {
            if (instance == null)
            {
                instance = value;
                DontDestroyOnLoad(value);
            }
            else if (instance != value)
            {
                Debug.Log($"{nameof(SettingsManager)} intance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public float sensitivity = 5000f;
    public int fov = 60;


    public void SetSensitivity(float level)
    {
        sensitivity = level;
        Debug.Log("sens: " + sensitivity);
    }

    public void SetFOV(float level)
    {
        fov = (int)level;
        Debug.Log("fov: " +  fov);
    }
}
