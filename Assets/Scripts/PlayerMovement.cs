using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform head;
    public Camera mainCam;
    public float sensitivity = .5f;
    public float clampAngle = 85;
    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    public float jumpForce = 10f;
    public float groundDrag = 10f;

    public LayerMask groundLayer;

    CapsuleCollider col;
    Rigidbody rb;
    float vertRot;
    float horRot;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vertRot = transform.localEulerAngles.x;
        horRot = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
    }

    public void Look()
    {
        float mouseHor = Input.GetAxis("Mouse X"); // Get horizontal mouse position
        float mouseVer = -Input.GetAxis("Mouse Y"); // Get vertical mouse position

        vertRot += mouseVer * sensitivity * Time.deltaTime; // Get vertical rotation based on mouse movement
        horRot += mouseHor * sensitivity * Time.deltaTime; // Get horizontal rotation based on mouse movement

        vertRot = Mathf.Clamp(vertRot, -clampAngle, clampAngle); // Limit how far up and down the player can look
        head.localRotation = Quaternion.Euler(vertRot, horRot, 0f); // Set rotation of head
    }

    public void Move()
    {
        float horMovement = Input.GetAxisRaw("Horizontal");
        float vertMovement = Input.GetAxisRaw("Vertical");

        // Get direction player is facing and move accordingly
        Quaternion yaw = Quaternion.Euler(0, head.eulerAngles.y, 0);
        Vector3 movement = yaw * new Vector3(horMovement * moveSpeed, 0, vertMovement * moveSpeed);

        rb.AddForce(movement.normalized * moveSpeed, ForceMode.Force);

        if (CheckIfGrounded())
        {
            rb.drag = groundDrag;

            if (Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            rb.drag = 0;
        }


        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    } 

    public bool CheckIfGrounded()
    {
        Vector3 start = col.transform.TransformPoint(col.center); // Get character's center
        float rayLength = col.height / 2 - col.radius + 0.05f; // Distance from 'start' to end of character + bit extra to detect ground

        bool hasHit = Physics.SphereCast(start, col.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer); // Detect if ground layer was found

        return hasHit;
    }
}
