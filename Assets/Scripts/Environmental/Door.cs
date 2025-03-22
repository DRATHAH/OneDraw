using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    public string triggerName;
    public float animationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator.speed = animationSpeed;
    }

    public void ActivateDoor(bool open)
    {
        animator.SetBool(triggerName, open);
    }
}
