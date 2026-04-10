using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    private float moveS;
    public float walkS;
    public float sprintS;

    public Transform lookDir;
    public Slider staminaBar;

    float horiInput;
    float vertInput;

    Vector3 moveDir;

    Rigidbody rb;

    public float heightOfPlayer;
    public LayerMask ground;
    bool onGround;

    public float dragOnGround;

    public float jumpForce;
    public float cooldownJump;
    public float inAirMulti;
    bool jumpable = true;

    public float currentStam;
    public float maxStam;
    public float stamDrain;
    public float stamRecharge;
    bool sprinting = true;

    public float maxSlope;
    private RaycastHit slopeHit;
    private bool slopeExit;

    public MovementState state;

    public AudioSource source;
    public float footstepsCooldown;
    public float footstepsDuration;
    public bool footstepsPlaying;

    public float speedSampleTime;
    bool readyToSample;
    Vector3 oldPosition;
    float lastSavedSpeed;
    public float speedThreshold;

    public TurnScript camScript;
    private float footstepsTimer;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dagger"), true);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Time.timeScale = 1f;
        readyToSample = true;
        footstepsTimer = 0f;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        onGround = Physics.Raycast(transform.position, Vector3.down, heightOfPlayer * 0.5f + 0.3f, ground);
        PlayerInput();
        PlayerState();
        SprintBarUpdate();
        HandleFootsteps();
        HandleVelocityToSpeed();

        staminaBar.value = currentStam;

        if (onGround)
        {
            rb.linearDamping = dragOnGround;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void FixedUpdate()
    {
        Movement();
        SpeedController();
    }

    private void StepClimb()
    {
        if (!onGround || moveDir.magnitude == 0)
        {
            return;
        }

        RaycastHit hitLower;

        float stepHeight = 0.5f;

        Vector3 origin = transform.position + Vector3.up * 0.05f;

        if (Physics.Raycast(origin, transform.forward, out hitLower, 0.6f))
        {
            Vector3 upperOrigin = transform.position + Vector3.up * stepHeight;

            if (!Physics.Raycast(upperOrigin, transform.forward, 0.6f))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 2f, rb.linearVelocity.z);
            }
        }
    }

    void PlayerInput()
    {
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && jumpable && onGround)
        {
            jumpable = false;
            Jump();
            Invoke(nameof(JumpReset), cooldownJump);
        }
    }

    private void PlayerState()
    {
        if (onGround && Input.GetKey(KeyCode.LeftShift) && currentStam > 0)
        {
            state = MovementState.sprinting;
            moveS = sprintS;
            sprinting = true;
            camScript.IsRunning = true;
            camScript.IsWalking = false;
        }
        else if (onGround)
        {
            state = MovementState.walking;
            moveS = walkS;
            sprinting = false;
            camScript.IsRunning = false;
            camScript.IsWalking = true;
        }
        else
        {
            state = MovementState.air;
            sprinting = false;
            camScript.IsRunning = false;
            camScript.IsWalking = false;
        }
    }

    private void SprintBarUpdate()
    {
        if (sprinting && currentStam > 0)
        {
            currentStam -= stamDrain * Time.deltaTime;
            currentStam = Mathf.Clamp(currentStam, 0, maxStam);
        }
        if (!sprinting && currentStam < maxStam)
        {
            currentStam += stamRecharge * Time.deltaTime;
            currentStam = Mathf.Clamp(currentStam, 0, maxStam);
        }
    }

    private void Movement()
    {
        moveDir = lookDir.forward * vertInput + lookDir.right * horiInput;

        if (onGround)
        {
            if (onGround)
            {
                Vector3 targetVelocity = moveDir.normalized * moveS;
                Vector3 velocity = rb.linearVelocity;

                Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0f, velocity.z);

                if (state == MovementState.sprinting)
                {
                    rb.linearVelocity = new Vector3(velocity.x + velocityChange.x,rb.linearVelocity.y,velocity.z + velocityChange.z);
                }
                else
                {
                    float acceleration = 1f;
                    rb.AddForce(velocityChange * acceleration, ForceMode.VelocityChange);
                }

                if (rb.linearVelocity.y <= 0)
                {
                    rb.AddForce(-Vector3.up * 3f, ForceMode.Force);
                }
            }
        }
        else if (!onGround)
        {
            rb.AddForce(moveDir.normalized * moveS * 10f * inAirMulti, ForceMode.Force);
        }

        if (PlayerOnSlope() && !slopeExit)
        {
            Vector3 slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
            slopeDir.y = 0f;
            slopeDir.Normalize();
            Vector3 targetVelocity = slopeDir * moveS;
            Vector3 velocity = rb.linearVelocity;

            Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0f, velocity.z);

            rb.AddForce(velocityChange, ForceMode.Force);
        }

        rb.useGravity = true;

        if (rb.linearVelocity.y > 5f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5f, rb.linearVelocity.z);
        }

        if (!onGround && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
    }

    private bool PlayerOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, heightOfPlayer * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlope && angle != 0;
        }

        return false;
    }

    private Vector3 PlayerMoveDirSlope()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    private void SpeedController()
    {
        if (PlayerOnSlope() && !slopeExit)
        {
            Vector3 velocityFlat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            float maxSpeed = moveS * 1.2f;

            if (velocityFlat.magnitude > maxSpeed)
            {
                Vector3 velocityLimit = velocityFlat.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(velocityLimit.x, rb.linearVelocity.y, velocityLimit.z);
            }
        }
        else
        {
            Vector3 velocityFlat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            float maxSpeed = moveS * 1.2f;

            if (velocityFlat.magnitude > maxSpeed)
            {
                Vector3 velocityLimit = velocityFlat.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(velocityLimit.x, rb.linearVelocity.y, velocityLimit.z);
            }
        }
    }

    private void Jump()
    {
        slopeExit = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        slopeExit = false;
        jumpable = true;
    }

    private void HandleFootsteps()
    {
        if (onGround && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            footstepsTimer -= Time.deltaTime;
            if (footstepsTimer <= 0 && !source.isPlaying)
            {
                source.Play();
                footstepsTimer = footstepsDuration;
            }
        }
        else
        {
            source.Stop();
            footstepsTimer = 0f;
        }
    }

    private void HandleVelocityToSpeed()
    {
        if (readyToSample)
        {
            readyToSample = false;
            StartCoroutine(SampleVelocity());
        }
    }

    private IEnumerator SampleVelocity()
    {
        yield return new WaitForSeconds(speedSampleTime);
        lastSavedSpeed = Mathf.Abs((transform.position - oldPosition).magnitude);
        oldPosition = transform.position;
        readyToSample = true;
    }

    public IEnumerator SpeedBoost(float amount, float duration)
    {
        walkS += amount;
        sprintS += amount;

        yield return new WaitForSeconds(duration);

        walkS -= amount;
        sprintS -= amount;
    }

}
