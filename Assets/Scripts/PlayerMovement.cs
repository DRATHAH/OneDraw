using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : DamageableCharacter
{
    #region Camera Movement Variables
    [Header("Camera Movement Variables")]
    public Transform head;
    public Camera mainCam;
    public float sensitivity = .5f;
    public float clampAngle = 85;

    float vertRot;
    float horRot;
    #endregion

    #region Walking Variables
    [Header("Walking Variables")]
    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    #endregion

    #region Jumping Variables
    [Header("Jumping Variables")]
    public float jumpForce = 10f;
    public float groundDrag = 10f;
    public float airDrag = 1;
    public float jumpDelay = 0.25f;

    bool canJump = true;
    #endregion

    #region Dashing Variables
    [Header("Dashing Variables")]
    public float dashSpeed = 20f;
    public float dashTime = 0.3f;
    public float dashCooldown = 0.75f;

    private bool canDash = true;
    private float dashCDCurrent;
    #endregion

    #region Reference Variables
    [Header("Reference Variables")]
    public Transform playerModel;
    public LayerMask groundLayer;
    public GameObject dashMeter;

    Slider dashSlider;
    CapsuleCollider col;
    Rigidbody body;
    #endregion

    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vertRot = transform.localEulerAngles.x;
        horRot = transform.localEulerAngles.y;

        dashSlider = dashMeter.GetComponent<Slider>();
        dashCDCurrent = dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Look();
        }
        Move();

        #region UI Elements

        if (!canDash)
        {
            dashCDCurrent += Time.deltaTime;
        }
        dashSlider.value = dashCDCurrent / dashCooldown;
        dashMeter.SetActive(dashCDCurrent < dashCooldown);

        #endregion
    }

    public void Look()
    {
        float mouseHor = Input.GetAxis("Mouse X"); // Get horizontal mouse position
        float mouseVer = -Input.GetAxis("Mouse Y"); // Get vertical mouse position

        vertRot += mouseVer * sensitivity * Time.deltaTime; // Get vertical rotation based on mouse movement
        horRot += mouseHor * sensitivity * Time.deltaTime; // Get horizontal rotation based on mouse movement

        vertRot = Mathf.Clamp(vertRot, -clampAngle, clampAngle); // Limit how far up and down the player can look
        head.localRotation = Quaternion.Euler(vertRot, horRot, 0f); // Set rotation of head

        playerModel.rotation = Quaternion.Euler(0, horRot, 0);
    }

    public void Move()
    {
        float horMovement = Input.GetAxisRaw("Horizontal");
        float vertMovement = Input.GetAxisRaw("Vertical");

        // Get direction player is facing and move accordingly
        Quaternion yaw = Quaternion.Euler(0, head.eulerAngles.y, 0);
        Vector3 movement = yaw * new Vector3(horMovement * moveSpeed, 0, vertMovement * moveSpeed);

        body.AddForce(movement.normalized * moveSpeed, ForceMode.Force);

        if (CheckIfGrounded())
        {
            body.drag = groundDrag;

            if (Input.GetKey(KeyCode.Space) && canJump)
            {
                canJump = false;
                body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
                body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(JumpDelay());
            }
        }
        else if (!CheckIfGrounded() && !body.useGravity)
        {
            body.drag = groundDrag;
            if (Input.GetKey(KeyCode.Space))
            {
                body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
                body.AddForce(transform.up * jumpForce, ForceMode.Force);
            }
        }
        else
        {
            body.drag = airDrag;
        }

        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Vector3 flatVel = new Vector3(body.velocity.x, 0f, body.velocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            body.velocity = new Vector3(limitedVel.x, body.velocity.y, limitedVel.z);
        }
    } 

    public bool CheckIfGrounded()
    {
        Vector3 start = col.transform.TransformPoint(col.center); // Get character's center
        float rayLength = col.height / 2 - col.radius + 0.05f; // Distance from 'start' to end of character + bit extra to detect ground

        bool hasHit = Physics.SphereCast(start, col.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer, QueryTriggerInteraction.Ignore); // Detect if ground layer was found

        return hasHit;
    }

    IEnumerator JumpDelay()
    {
        yield return new WaitForSeconds(jumpDelay);
        canJump = true;
    }
    
    IEnumerator Dash()
    {
        body.AddForce(new Vector3(playerModel.forward.x * dashSpeed, 0f, playerModel.forward.z * dashSpeed), ForceMode.Impulse);
        yield return new WaitForSeconds (dashTime);

        dashCDCurrent = 0;
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);

        dashCDCurrent = dashCooldown;
        canDash = true;
    }
}
