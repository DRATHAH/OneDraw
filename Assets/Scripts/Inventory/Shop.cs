using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    public UnityEvent buyEvent;
    public UnityEvent brokeEvent;

    public void Buy(int amount)
    {
        if (PlayerStats.Instance.coins - amount >= 0)
        {
            PlayerStats.Instance.UpdateCoins(-amount);
            buyEvent.Invoke();
        }
        else
        {
            brokeEvent.Invoke();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
