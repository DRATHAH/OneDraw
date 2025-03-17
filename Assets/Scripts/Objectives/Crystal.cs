using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Pickup
{
    private CrystalGate gate;

    private void Start()
    {
        gate = GetComponentInParent<CrystalGate>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.CompareTag("Arrow"))
            gate.ShatterCrystal(gameObject);
    }
}
