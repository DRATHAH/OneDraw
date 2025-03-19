using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        loot = 1,
        crystal = 2,
    }
    public PickupType type = PickupType.loot;
    [Tooltip("Set true if the pickup is supposed to do something upon being picked up. Ex: a coin immediately gives a player more gold.")]
    public bool useUponCollection = false;
    [Tooltip("The item the player will receive/use upon collection.")]
    public Item item;
    [Tooltip("How many items the player will receive/use upon collection.")]
    public int value = 1;
    [Tooltip("How fast the item will rotate on the ground.")]
    public float rotSpeed = 50;
    [Tooltip("How fast the item will move up and down on the ground.")]
    public float bobSpeed = 1;
    [Tooltip("How far the item will move up and down on the ground.")]
    public float bobAmount = 0.025f;
    [Tooltip("The model that moves on the ground.")]
    public Transform graphic;


    bool canDestroy = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.transform.GetComponent<PlayerMovement>()) // Make sure collider is player
        {
            for (int i = 0; i < value; i++) // Go through how many items this pickup contains
            {
                if (!useUponCollection && item && Inventory.instance.Add(item)) // If there's space for the item, pick it up
                {
                    Debug.Log("Picked up " + item.name);
                }
                else if (useUponCollection && item) // Or if the player uses it upon collection
                {
                    item.Use();
                }
            }

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        graphic.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.World);
        graphic.position = transform.position + new Vector3(0, bobAmount * Mathf.Sin(Time.time * bobSpeed), 0f);
    }
}
