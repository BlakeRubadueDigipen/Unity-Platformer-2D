using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [Tooltip("Translates the character with interpolation")]
    public float movementSpeed = 7.5f;

    [Header("Jumping")]
    [Tooltip("Sets the velocity to the jumpVelocity")]
    public float jumpVelocity = 10.0f;
    [Tooltip("The number of jumps")]
    public int jumpCount = 2;
    private int jumpsRemaining;
    [Tooltip("X and Y are the position of the boxcast, Z and W are the width and height of the boxcast.")]
    public Vector4 groundDetection;
    [Tooltip("The layers upon which the boxcast are cast")]
    public LayerMask groundMask;

    [Header("Ground Slam")]
    [Tooltip("Sets the velocity to the groundSlamForce")]
    public float groundSlamForce = 25.0f;
    [Tooltip("Is the player currently groundSlamming?")]
    public bool isSlamming = true;
    private bool wasSlamming = true;
    [Tooltip("The particle system to be instantiated upon collision")]
    public ParticleSystem groundSlamParticles;

    [Header("Dash")]
    [Tooltip("Sets the velocity to the dashForce")]
    public float dashForce = 15f;
    [Tooltip("Time between keystrokes required to register as a dash")]
    public float dashTime = 0.25f;
    [Tooltip("Direction of the keypress: 0 = left, 1 = right, -1 = null")]
    public int dashDirection = -1;
    [SerializeField]
    [Tooltip("In seconds")]
    private float timeSinceLastKeyPress_Dash = 0;

    //Local components
    private Rigidbody2D rb;
    private CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); //Set the rb variable to the RigidBody2D attached to the player gameObject.
        impulseSource = gameObject.GetComponent<CinemachineImpulseSource>();
        jumpsRemaining = jumpCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xVelocity = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        transform.Translate(new Vector2(xVelocity, 0));
    }

    private void Update()
    {
        if (isGrounded() && Input.GetButton("Jump"))
        {
            Jump(jumpVelocity);
        }
        //Double Jump
        if (!isGrounded() && Input.GetButtonDown("Jump") && jumpsRemaining > 1)
        {
            Jump(jumpVelocity);
            jumpsRemaining -= 1;
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isGrounded())
        {
            isSlamming = true;
            Jump(-groundSlamForce);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (dashDirection == 1 && timeSinceLastKeyPress_Dash < dashTime)
            {
                rb.velocity = new Vector2(dashForce, rb.velocity.y);
            }
            dashDirection = 1;
            timeSinceLastKeyPress_Dash = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (dashDirection == 0 && timeSinceLastKeyPress_Dash < dashTime)
            {
                rb.velocity = new Vector2(-dashForce, rb.velocity.y);
            }
            dashDirection = 0;
            timeSinceLastKeyPress_Dash = 0;
        }

        timeSinceLastKeyPress_Dash += Time.deltaTime;

        

        if (isGrounded() && wasSlamming)
        {
            impulseSource.GenerateImpulseAt(transform.position, new Vector3(0, rb.velocity.y, 0));
            ParticleSystem gSlam_Particles = Instantiate(groundSlamParticles, new Vector3(transform.position.x, transform.position.y -0.5f, -5), transform.rotation);
            wasSlamming = false;
        }

    }

    private void LateUpdate()
    {
        if (isSlamming)
        {
            wasSlamming = true;
        }
    }

    public bool isGrounded()
    {
        if(Physics2D.OverlapBox(new Vector2(transform.position.x + groundDetection.x, transform.position.y + groundDetection.y),
            new Vector2(groundDetection.z, groundDetection.w), 0, groundMask))
        {
            jumpsRemaining = jumpCount;
            isSlamming = false;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void Jump(float force)
    {
        //rb.AddForce(new Vector2(0, force));
        rb.velocity = new Vector2(0, force);
    }


    
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(transform.position.x + groundDetection.x, transform.position.y + groundDetection.y, transform.position.z), new Vector3(groundDetection.z, groundDetection.w));
    }

}
