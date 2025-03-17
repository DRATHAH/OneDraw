using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        coin = 1,
        health = 2,
        crystal
    }
    public PickupType type = PickupType.coin;
    public int value = 1;

    public float rotSpeed = 1;
    public float bobSpeed = 1;
    public Transform graphic;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (type == PickupType.coin)
            {
                PlayerStats.Instance.coins += value;
            }
            else if (type == PickupType.health)
            {
                other.transform.root.GetComponent<PlayerMovement>().Heal(value);
            }

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        graphic.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.World);
        graphic.position = (transform.position + Vector3.up * 0.5f) + new Vector3(0, .025f * Mathf.Sin(Time.time * bobSpeed), 0f);
    }
}
