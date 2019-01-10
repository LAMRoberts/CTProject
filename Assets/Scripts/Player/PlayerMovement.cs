using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement
    private Rigidbody rb;
    public Transform forwards;
    public Transform leftwards;
    public Transform rightwards;
    public Transform backwards;
    Vector3 velocity;

    // stamina
    public float walkingSpeed = 10.0f;
    public float runningSpeed = 20.0f;
    private float moveSpeed = 0.0f;
    public float maxStamina = 100.0f;
    public float stamina = 100.0f;
    private float staminaUsed = 0.0f;
    public float staminaRate = 1.0f;
    public float staminaCooldown = 1.5f;
    private float timeSinceAction = 0.0f;

    // looking
    public float minAngle;
    public float maxAngle;
    public float horizontalSensitivity;
    public float verticalSensitivity;       
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Camera playerCam;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update ()
    {
        // lock cursor
        if (Input.GetKeyDown("l"))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        Stamina();

        MovePlayer();

        MoveCamera();
    }

    void Stamina()
    {
        // if running
        if (Input.GetKey("left shift") && Input.GetKey("w") && stamina > staminaRate)
        {
            moveSpeed = runningSpeed;
            staminaUsed = staminaRate;
        }
        else
        {
            moveSpeed = walkingSpeed;
            staminaUsed = 0.0f;
        }

        ConsumeStamina(staminaUsed);

        if (timeSinceAction < staminaCooldown)
        {
            timeSinceAction += Time.deltaTime;
        }
        else
        {
            if (stamina < (maxStamina - staminaRate))
            {
                stamina += staminaRate;
            }
            else
            {
                stamina = maxStamina;
            }
        }
    }

    public void ConsumeStamina(float value)
    {
        if (value > 0.0f)
        {
            if (stamina > value)
            {
                stamina -= value;
            }
            else
            {
                stamina = 0.0f;
            }

            timeSinceAction = 0.0f;
        }
    }

    void MovePlayer()
    {
        velocity = new Vector3(0, rb.velocity.y, 0);

        // if moving forward
        if (Input.GetKey("w"))
        {
            velocity += new Vector3(transform.forward.x * moveSpeed, 0, transform.forward.z * moveSpeed);
        }

        // if moving backwards
        if (Input.GetKey("s"))
        {
            velocity += new Vector3(-(transform.forward.x) * moveSpeed, 0, -(transform.forward.z) * moveSpeed);
        }

        // if moving left
        if (Input.GetKey("a"))
        {
            velocity += new Vector3(-(transform.right.x) * moveSpeed, 0, -(transform.right.z * moveSpeed));
        }

        // if moving right
        if (Input.GetKey("d"))
        {
            velocity += new Vector3(transform.right.x * moveSpeed, 0, transform.right.z * moveSpeed);
        }

        rb.velocity = velocity;
    }

    void MoveCamera()
    {
        yaw += horizontalSensitivity * Input.GetAxis("Mouse X");
        pitch = Mathf.Clamp(pitch - (verticalSensitivity * Input.GetAxis("Mouse Y")), minAngle, maxAngle);

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        playerCam.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
