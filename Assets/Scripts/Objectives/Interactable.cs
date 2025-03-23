using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public SphereCollider col;
    [Tooltip("Layer player is on.")]
    public LayerMask playerLayer;
    [Tooltip("Actual interactable model, used for distance calculations.")]
    public Transform interactable;
    [Tooltip("Events to run when interactings.")]
    public UnityEvent interactEvent;
    [Tooltip("Event that happens when you exit interactables.")]
    public UnityEvent leaveEvent;
    [Tooltip("Event that happens when an interactable is activateds.")]
    public UnityEvent activateEvent;
    [Tooltip("Canvas that says 'you can interact with this'")]
    public GameObject promptCanvas;
    [Tooltip("Speed at which canvas turns to face player")]
    public float smoothSpeed = 5;

    float promptYVelocity;

    bool hasPlayer = false; // Whether player is inside interaction range
    Transform player;
    bool canInteract = true; // Whether player can interact or not
    bool isFacing = false; // Whether player is facing interactable or not
    bool interacting = false; // Whether player is currently interacting

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.playerCamera.transform;
        promptCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && canInteract && !interacting)
        {
            hasPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && canInteract)
        {
            hasPlayer = false;
            promptCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayer && canInteract && !interacting)
        {
            Vector3 direction = (interactable.position - player.position).normalized; // Gets direction player is from interactable
            Ray directRay = new Ray(player.position, direction);
            RaycastHit obstacle;
            if (Physics.Raycast(directRay, out obstacle, col.radius * 2f, ~playerLayer, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Dot(player.transform.TransformDirection(Vector3.forward), direction) > 0.5f) // Makes sure player is somewhat facing interactable
                {
                    promptCanvas.SetActive(true);
                    isFacing = true;

                    // Make prompt canvas face player
                    float targetYRot = player.transform.eulerAngles.y;
                    float currentYRot = promptCanvas.transform.rotation.eulerAngles.y;
                    float newYAngle = Mathf.SmoothDampAngle(currentYRot, targetYRot, ref promptYVelocity, Time.deltaTime * smoothSpeed);
                    float xAngle = player.transform.eulerAngles.x;
                    promptCanvas.transform.eulerAngles = new Vector3(xAngle, newYAngle, promptCanvas.transform.eulerAngles.z);
                }
                else
                {
                    promptCanvas.SetActive(false);
                    isFacing = false;
                }
            }
        }

        if (!interacting && canInteract && Input.GetKeyDown(KeyCode.E) && hasPlayer && isFacing)
        {
            Interact();
            promptCanvas.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!interacting)
        {
            interacting = true;
            interactEvent.Invoke();
            Debug.Log("Interacted with " + gameObject.name);
        }
        else
        {
            interacting = false;
            leaveEvent.Invoke();
            Debug.Log("Stopping interaction");
        }
    }

    public void SetInteractableActive(bool state)
    {
        canInteract = state;
    }
}