using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] Transform grapplePivot;
    [SerializeField] LayerMask grappleableLayers;

    [SerializeField] bool useMaxDistance;
    [SerializeField] float maxDistance;
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
    [HideInInspector] public bool hookLaunched;
    [HideInInspector] public bool isReeling;
    public float springDistance;
    private float timeSinceUnhooked;
    [HideInInspector] public bool isHookBeingLaunched;
    [HideInInspector] public float hookLaunchDistanceTraveled; // For keeping track of how far the hook has travled between frames when it's being launched.
    [HideInInspector] public GameObject grappledObject;
    private bool doChainBreak;
    private float chainBreakTimeTracker;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
    }

    // Clean this up a bit later. Currently a lot of functionality from the SwitchHook() function was put here and there as part of making the chain visually extend when grappling.
    // Perhaps make a new function with similar functionality to the SwitchHookState() function but for the in-between part where the hook is being launched.
    private void Update()
    {
        if (GameManager.playerIsDead) // Prevent input if the player has died (to avoid post-mortem shenanigans)
        {
            return;
        }

        //Debug.Log(hookLaunchDistanceTraveled);
        if (isHookBeingLaunched)
        {
            hookLaunchDistanceTraveled += hookLaunchSpeed * Time.deltaTime;
            if(Vector2.Distance(transform.position, grapplePoint) < hookLaunchDistanceTraveled)
            {
                isHookBeingLaunched = false;
                hookLaunchDistanceTraveled = 0;
                springDistance = Vector2.Distance(transform.position, grapplePoint);
                SwitchHookState();
            }
            else if(hookLaunchDistanceTraveled > maxDistance) // If the hook has extended further than the maximum distance, the grapple has failed.
            {
                isHookBeingLaunched = false;
                hookLaunchDistanceTraveled = 0;
            }
        }

        if (!hookLaunched)
        {
            timeSinceUnhooked += Time.deltaTime;
        }

        if(hookLaunched && !doChainBreak)
        {
            doChainBreak = CheckChainBreak();
        }

        if (doChainBreak)
        {
            chainBreakTimeTracker += Time.deltaTime;
        }

        if(doChainBreak && chainBreakTimeTracker > chainBreakTime)
        {
            doChainBreak = false;
            chainBreakTimeTracker = 0;
            SwitchHookState();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isHookBeingLaunched) // Don't run the SetGrapplePoint function if the player is currently trying to launch the hook
            {
                if (SetGrapplePoint() && timeSinceUnhooked >= hookCooldown)
                {
                    // Between the time that the grapple point is set and the hook state is changed, there should be played an animation that delays the SwitchHookState function.
                    // This delay should depend on a variable that determines the hook launch speed.
                    // Essentially we want to emulate a projectile hook without actually using physics.

                    // Solution: Make a bool that is set here instead of calling SwitchHookState().
                    // The bool will be checked in an if-statement at the top of update, and update a timer that counts down until it then calls SwitchHookState from there.

                    //SwitchHookState();
                    grapplingHook.transform.position = grapplePoint;
                    grapplingHook.transform.parent = hookLaunched ? null : transform;
                    isHookBeingLaunched = true;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0) && hookLaunched)
        {
            SwitchHookState();
        }
        else if(Input.GetMouseButtonUp(0) && isHookBeingLaunched)
        {
            hookLaunchDistanceTraveled = 0;
            isHookBeingLaunched = false;
        }
        if (Input.GetMouseButton(1))
        {
            springDistance = Mathf.Clamp(springDistance - Time.deltaTime * reelingSpeed, 0.05f, maxDistance);
            springJoint.distance = springDistance;
            isReeling = true;
        }
        else
        {
            isReeling = false;
        }
    }

    private bool SetGrapplePoint() // Returns if the grapple was successful
    {
        bool successfulGrapple = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 grappleDirection = mousePos - (Vector2)grapplePivot.position;
        Vector2 tempGrapplePoint = mousePos;
        RaycastHit2D hit = Physics2D.Raycast(grapplePivot.position, grappleDirection, useMaxDistance ? maxDistance : Mathf.Infinity, grappleableLayers);
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
}
