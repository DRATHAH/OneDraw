using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLook : MonoBehaviour
{
    public LayerMask playerLayer;
    [Tooltip("Neck of the character.")]
    public Transform looker;
    [Tooltip("Speed at which character's head turns to face player")]
    public float smoothSpeed = 5;

    Quaternion originalRotation;
    bool hasPlayer = false;
    Transform player;
    SphereCollider col;
    float promptYVelocity;

    private void Start()
    {
        player = GameManager.instance.player.transform;
        col = GetComponent<SphereCollider>();
        originalRotation = looker.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageableCharacter player = other.GetComponentInParent<DamageableCharacter>();
        if (player && player.isPlayer)
        {
            hasPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DamageableCharacter player = other.GetComponentInParent<DamageableCharacter>();
        if (player && player.isPlayer)
        {
            hasPlayer = false;
        }
    }

    private void Update()
    {
        if (hasPlayer)
        {
            Vector3 direction = (player.position - transform.position).normalized; // Gets direction player is from interactable
            Ray directRay = new Ray(transform.position, direction);
            RaycastHit obstacle;
            if (Physics.Raycast(directRay, out obstacle, col.radius * 2f, ~playerLayer, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Dot(transform.TransformDirection(Vector3.forward), direction) > 0.5f) // Makes sure interactable is somewhat facing player
                {
                    // Make head face player
                    Vector3 faceDirection = (transform.position - player.position).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(faceDirection);
                    looker.rotation = Quaternion.Slerp(looker.rotation, lookRot, Time.deltaTime * smoothSpeed);
                }
                else
                {
                    looker.rotation = Quaternion.Slerp(looker.rotation, originalRotation, Time.deltaTime * smoothSpeed);
                }
            }
        }
        else
        {
            looker.rotation = Quaternion.Slerp(looker.rotation, originalRotation, Time.deltaTime * smoothSpeed);
        }
    }
}
