using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        coin = 1,
        health = 2,
        crystal = 3,
        loot = 4,
    }
    public PickupType type = PickupType.coin;
    public int value = 1;

    public float rotSpeed = 1;
    public float bobSpeed = 1;
    public float bobAmount = 0.025f;
    public Transform graphic;

    [Header("Only if 'type' is loot")]
    public Item item;


    bool canDestroy = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.transform.GetComponent<DamageableCharacter>())
        {
            if (type == PickupType.coin)
            {
                if (PlayerStats.Instance)
                {
                    PlayerStats.Instance.coins += value;
                    Destroy(gameObject);
                }
            }
            else if (type == PickupType.health)
            {
                other.transform.root.GetComponent<PlayerMovement>().Heal(value);
                Destroy(gameObject);
            }
            else if (type == PickupType.loot && Inventory.instance.Add(item))
            {
                Debug.Log("Picked up " + item.name);
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        graphic.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.World);
        graphic.position = transform.position + new Vector3(0, bobAmount * Mathf.Sin(Time.time * bobSpeed), 0f);
    }
}
