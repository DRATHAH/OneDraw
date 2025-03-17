using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGate : MonoBehaviour
{
    public float timeLimit = 10f;

    private GameObject[] crystals;
    private int ogCrystalCount;
    private int currCrystalCount;
    private bool hasCrystals = true;

    private void Start()
    {
        FindCrystals();
    }

    private void Update()
    {
        if(currCrystalCount == 0)
        {
            // Open gate once all crystals are shattered
            Debug.Log("All crystals shattered!");
            Destroy(gameObject);
        }
    }

    private void FindCrystals()
    {
        // Populate crystal array with the children of the Crystal Gate
        ogCrystalCount = transform.childCount;
        currCrystalCount = ogCrystalCount;
        crystals = new GameObject[ogCrystalCount];

        for (int i = 0; i < ogCrystalCount; i++)
            crystals[i] = transform.GetChild(i).gameObject;
    }
    public void ShatterCrystal(GameObject crystal)
    {
        // If the first crystal has been shattered, start the timer
        if (ogCrystalCount == currCrystalCount)
        {
            Invoke(nameof(ResetCrystals), timeLimit);
        }
        crystal.SetActive(false);
        currCrystalCount--;
    }

    private void ResetCrystals()
    {
        // If not all crystals are shattered within the time limit, reset all crystals
        Debug.Log("Time Limit reached. Reset crystals.");
        for (int i = 0; i < ogCrystalCount; i++)
            crystals[i].SetActive(true);

        currCrystalCount = ogCrystalCount;
        
    }
}
