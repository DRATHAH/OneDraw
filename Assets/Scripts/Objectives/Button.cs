using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    public GameObject button; // Graphic of the button
    public Transform upPos, downPos; // Unpressed and pressed positions

    bool isPressed = false;
    public GameObject presser; // Thing pressing button to make sure it's off
    AudioSource sound; // Sound that plays when button is pressed

    public UnityEvent onPressed, onReleased; // Functions that run depending on the state of the button

    // Start is called before the first frame update
    void Start()
    {
        //sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && !other.gameObject.name.Equals(gameObject.name)) // Makes sure button doesn't collide with itself
        {
            isPressed = true;
            button.transform.position = downPos.position;
            //sound.Play();
            presser = other.gameObject;
            onPressed.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser) // Makes sure button doesn't collide with itself
        {
            isPressed = false;
            button.transform.position = upPos.position;
            onReleased.Invoke();
        }
    }

    private void Update()
    {
        if (presser == null)
        {
            isPressed = false;
            button.transform.position = upPos.position;
            onReleased.Invoke();
        }
    }
}
