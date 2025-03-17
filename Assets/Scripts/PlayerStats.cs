using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Allows script to be directly referenced without variables and saves it across scenes
    #region Singleton

    private static PlayerStats instance;
    public static PlayerStats Instance
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
                Debug.Log($"{nameof(PlayerStats)} intance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public int coins = 0;
    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
