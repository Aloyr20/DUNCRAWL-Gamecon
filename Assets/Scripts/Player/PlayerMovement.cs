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

    }

    void Update()
    {
        onGround = Physics.Raycast(transform.position, Vector3.down, heightOfPlayer * 0.5f + 0.2f, ground);
        PlayerInput();
        SpeedController();
        PlayerState();
        SprintBarUpdate();

        StartCoroutine(VelocityToSpeed());
        StartCoroutine(Footsteps());

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
        }
        else if (onGround)
        {
            state = MovementState.walking;
            moveS = walkS;
            sprinting = false;
        }
        else
        {
            state = MovementState.air;
            sprinting = false;
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
            rb.AddForce(moveDir.normalized * moveS * 10f, ForceMode.Force);
        }
        else if (!onGround)
        {
            rb.AddForce(moveDir.normalized * moveS * 10f * inAirMulti, ForceMode.Force);
        }

        if (PlayerOnSlope() && !slopeExit)
        {
            rb.AddForce(PlayerMoveDirSlope() * moveS * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        rb.useGravity = !PlayerOnSlope();

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
            if (rb.linearVelocity.magnitude > moveS)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveS;
            }
        }
        else
        {
            Vector3 velocityFlat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (velocityFlat.magnitude > moveS)
            {
                Vector3 velocityLimit = velocityFlat.normalized * moveS;
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

    public IEnumerator Footsteps()
    {
        if (onGround && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (!source.isPlaying)
            {
                source.Play();
                yield return new WaitForSeconds(footstepsDuration);
            }
        }

        else
        {
            source.Stop();
            StopCoroutine(Footsteps());
        }
    }


    public IEnumerator VelocityToSpeed()
    {
        if (readyToSample)
        {
            readyToSample = false;

            yield return new WaitForSeconds(speedSampleTime);

            lastSavedSpeed = Mathf.Abs((transform.position - oldPosition).magnitude);

            oldPosition = transform.position;

            readyToSample = true;
        }
    }

}
