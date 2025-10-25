using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] LayerMask ground;
    [SerializeField] float walkSpeed;
    [Range(0, 2)][SerializeField] float inAirSpeedMult; // Will apply if the player is in the air, and is not hooked
    [Range(0, 2)][SerializeField] float hookedInAirSpeedMult; // Will apply if the player is in the air, and is hooked
    [SerializeField] bool useGroundSpeedLimit;
    [SerializeField] float maxSpeed;
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    private GrapplingHook hookScript;
    [SerializeField] GameObject grapplingHook;
    [HideInInspector] public bool isWalking;
    private Vector2 playerInput;
    private float originalDampingRatio;
    [HideInInspector] public bool isGrounded;
    private CircleCollider2D playerCollider;
    [SerializeField] bool canJump;
    [SerializeField] float jumpForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        hookScript = GetComponent<GrapplingHook>();
        playerCollider = GetComponent<CircleCollider2D>();
        originalDampingRatio = springJoint.dampingRatio;
    }

    private void Update()
    {
        if (GameManager.playerIsDead)
        {
            return;
        }

        isGrounded = IsGrounded();

        GetMovementInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Make it possible for the player to walk freely while they are walking around on the ground with the hook launched
        if (hookScript.hookLaunched && isWalking && !hookScript.isReeling && isGrounded)
        {
            // If the hook is launched, and the player is walking, and the player is not reeling, and the player is on the ground
            springJoint.distance = Vector2.Distance(transform.position, grapplingHook.transform.position);
            springJoint.dampingRatio = 0;
        }
        else
        {
            springJoint.dampingRatio = originalDampingRatio;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    public bool IsGrounded(float maxDistanceToGround)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, transform.localScale.x * 0.5f, Vector2.down, maxDistanceToGround, ground);
        if (GameManager.debugMode)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.down * maxDistanceToGround, Color.red, Time.deltaTime);
            if (hit.collider != null)
            {
                Debug.Log($"IsGrounded hit {hit.collider.name} on layer {hit.collider.gameObject.layer}.");
            }
        }
        return hit;
    }

    public bool IsGrounded()
    {
        // Setting the distance to ground to be the radius of the player collider, and then a little extra. This only works because the player is round.
        return IsGrounded(playerCollider.radius * 1.1f);
    }

    private void ApplyMovement()
    {
        rb.AddForce(playerInput * walkSpeed * Time.fixedDeltaTime); // We apply movement as physics, to make it more consistent with the grappling hook
        if (useGroundSpeedLimit && isGrounded)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        }

        if (GameManager.debugMode)
        {
            Debug.DrawLine(transform.position, (Vector2)transform.position + rb.linearVelocity, Color.yellow, Time.fixedDeltaTime);
        }
    }

    private void GetMovementInput()
    {
        playerInput = (Input.GetAxisRaw("Horizontal") * Vector2.right + (isGrounded ? Vector2.zero : hookScript.hookLaunched ? Input.GetAxisRaw("Vertical") * Vector2.up : Vector2.zero)).normalized;
        // playerInput is set to a Vector2. The x value is 1 when pressing D and -1 when pressing A. The y value is 1 when pressing W, and -1 when pressing S.
        // However the y value is only 1 or -1 if the player is hooked while not on the ground. Otherwise it's 0.
        // Essentially this just means that the player can move around in all directions when they're suspended on the hook, but otherwise can only go left or right.
        if (!isGrounded && hookScript.hookLaunched)
        {
            playerInput *= hookedInAirSpeedMult;
        }
        else if (!isGrounded)
        {
            playerInput *= inAirSpeedMult;
        }

        if (playerInput != Vector2.zero)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void Jump()
    {
        if (!IsGrounded(playerCollider.radius * 1.1f + 0.3f) || !canJump)
        {
            return;
        }
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    
}
