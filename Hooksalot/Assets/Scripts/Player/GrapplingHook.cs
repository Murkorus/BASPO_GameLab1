using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LayerMask grappleableLayers;

    public float maxDistance;
    public float distanceWhenHooked;
    [SerializeField] float reelingSpeed;
    [SerializeField] float hookCooldown;
    [SerializeField] float hookLaunchSpeed; // How many units per second does the hook travel while in the process of being launched?
    [SerializeField] float chainBreakForce;
    [SerializeField] float chainBreakTime; // How long can the chain stay attached before it breaks when a critical force has been applied?

    [Header("References")]
    [SerializeField] GameObject grapplingHook;
    private SpringJoint2D springJoint;
    private Rigidbody2D rb;
    

    private Vector2 grapplePoint;
    public float springDistance;
    private float timeSinceUnhooked;
    [HideInInspector] public bool hookLaunched;
    [HideInInspector] public bool isReeling;
    [HideInInspector] public bool isHookBeingLaunched;
    [HideInInspector] public int reelDirection; // 1 if unreeling (Chain gets longer), 0 if standing still (reached max length), -1 if reeling in (chain gets shorter)
    [HideInInspector] public float hookLaunchDistanceTraveled; // For keeping track of how far the hook has travled between frames when it's being launched.
    [HideInInspector] public GameObject grappledObject;
    private float originalDampingRatio;
    private bool doChainBreak;
    private float chainBreakTimeTracker;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        originalDampingRatio = springJoint.dampingRatio;
    }

    // Clean this up a bit later. Currently a lot of functionality from the SwitchHook() function was put here and there as part of making the chain visually extend when grappling.
    // Perhaps make a new function with similar functionality to the SwitchHookState() function but for the in-between part where the hook is being launched.
    private void Update()
    {
        if (GameManager.playerIsDead) // Prevent input if the player has died (to avoid post-mortem shenanigans)
        {
            return;
        }

        if (!hookLaunched) // If the hook is not currently attached to something, count down for the next time the player can launch the hook.
        {
            timeSinceUnhooked += Time.deltaTime;
        }

        if (isHookBeingLaunched) // If the hook launching animation should be playing, do the logic for extending the hook chain.
        {
            ExtendHookChain();
        }

        if (hookLaunched && !doChainBreak) // If the hook is currently attached to something, determine if the chain break process should begin.
        {
            doChainBreak = CheckChainBreak();
        }

        if (doChainBreak) // Counts down to when the chain should break after the breaking threshhold has been reached.
        {
            chainBreakTimeTracker += Time.deltaTime;
        }

        if (doChainBreak && chainBreakTimeTracker > chainBreakTime) // If the chainbreak process has started and the countdown finished, finally break the chain.
        {
            doChainBreak = false;
            chainBreakTimeTracker = 0;
            SwitchHookState();
        }

        /* ### Inputs ### */

        // Launch the hook when pressing left click.
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHookBeingLaunched) // Don't run the SetGrapplePoint function if the player is currently trying to launch the hook
            {
                if (SetGrapplePoint() && timeSinceUnhooked >= hookCooldown)
                {
                    grapplingHook.transform.position = grapplePoint;
                    grapplingHook.transform.parent = hookLaunched ? null : transform;
                    isHookBeingLaunched = true;
                }
            }
        }

        // Retract the hook when left click is released.
        if (Input.GetMouseButtonUp(0) && hookLaunched)
        {
            SwitchHookState();
        }
        else if(Input.GetMouseButtonUp(0) && isHookBeingLaunched)
        {
            hookLaunchDistanceTraveled = 0;
            isHookBeingLaunched = false;
        }

        // Reel in the player when pressing right cick.
        if (Input.GetMouseButtonDown(1))
        {
            reelDirection = -1;
            isReeling = true;
        }
        
        if(Input.GetMouseButtonUp(1))
        {
            reelDirection = 1;
            isReeling = true;
        }

        if (isReeling)
        {
            if(reelDirection == -1)
            {
                springDistance = Mathf.Clamp(springDistance - Time.deltaTime * reelingSpeed, 0.05f, distanceWhenHooked);
            }
            else if(reelDirection == 1)
            {
                Debug.Log("B");
                springDistance = Mathf.Clamp(Vector2.Distance(grapplePoint, transform.position), 0.05f, distanceWhenHooked);
                springJoint.dampingRatio = 0;
            }
            springJoint.distance = springDistance;

            if (springDistance <= 0.05f || springDistance >= distanceWhenHooked || hookLaunched == false)
            {
                Debug.Log("C");
                isReeling = false;
                reelDirection = 0;
                springJoint.dampingRatio = originalDampingRatio;
            }
        }
    }

    private bool SetGrapplePoint() // Returns if the grapple was successful
    {
        bool successfulGrapple = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 grappleDirection = mousePos - (Vector2)transform.position;
        Vector2 tempGrapplePoint = mousePos;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, grappleDirection, maxDistance, grappleableLayers);
        if (hit)
        {
            tempGrapplePoint = hit.point;
            successfulGrapple = true;
            grappledObject = hit.collider.gameObject;
        }
        else
        {
            grappledObject = null;
        }
        grapplePoint = tempGrapplePoint;
        springDistance = Vector2.Distance(transform.position, grapplePoint);
        return successfulGrapple;
    }

    public void SwitchHookState()
    {
        // Switch between launched and retracted states
        timeSinceUnhooked = 0;
        distanceWhenHooked = Vector2.Distance(transform.position, grapplePoint);
        hookLaunched = !hookLaunched;
        springJoint.enabled = hookLaunched;
        springJoint.distance = springDistance;
        grapplingHook.transform.position = grapplePoint;
        grapplingHook.transform.parent = hookLaunched ? null : transform;
    }

    // Returns whether or not the chain should break
    private bool CheckChainBreak()
    {
        Vector2 chainVector = (Vector2)transform.position - grapplePoint; // Vector pointing from the grapple point to the player, magnitude is length between those points.
        chainVector.Normalize();
        // Calculate dot product of player velocity and chainVector to find out if the chain should break
        float dotProduct = rb.linearVelocity.x * chainVector.x + rb.linearVelocity.y * chainVector.y;
        if (dotProduct > chainBreakForce)
        {
            if (GameManager.debugMode)
            {
                Debug.Log($"Chain broke at velocity {rb.linearVelocity.magnitude}, as it exceeded the limit of {chainBreakForce}.");
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ExtendHookChain()
    {
        hookLaunchDistanceTraveled += hookLaunchSpeed * Time.deltaTime;
        if (Vector2.Distance(transform.position, grapplePoint) < hookLaunchDistanceTraveled)
        {
            isHookBeingLaunched = false;
            hookLaunchDistanceTraveled = 0;
            springDistance = Vector2.Distance(transform.position, grapplePoint);
            SwitchHookState();
        }
        else if (hookLaunchDistanceTraveled > maxDistance) // If the hook has extended further than the maximum distance, the grapple has failed.
        {
            isHookBeingLaunched = false;
            hookLaunchDistanceTraveled = 0;
        }
    }
}
