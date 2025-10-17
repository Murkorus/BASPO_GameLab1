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

    [Header("References")]
    [SerializeField] GameObject grapplingHook;
    private SpringJoint2D springJoint;
    

    private Vector2 grapplePoint;
    [HideInInspector] public bool hookLaunched;
    [HideInInspector] public bool isReeling;
    public float springDistance;
    private float timeSinceUnhooked;
    [HideInInspector] public bool isHookBeingLaunched;
    [HideInInspector] public float hookLaunchDistanceTraveled; // For keeping track of how far the hook has travled between frames when it's being launched.

    private void Start()
    {
        springJoint = GetComponent<SpringJoint2D>();
    }

    private void Update()
    {
        Debug.Log(hookLaunchDistanceTraveled);
        if (isHookBeingLaunched)
        {
            hookLaunchDistanceTraveled += hookLaunchSpeed * Time.deltaTime;
            if(Vector2.Distance(transform.position, grapplePoint) < hookLaunchDistanceTraveled)
            {
                isHookBeingLaunched = false;
                hookLaunchDistanceTraveled = 0;
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

        if (GameManager.playerIsDead)
        {
            return;
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

                    SwitchHookState();
                    //isHookBeingLaunched = true;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0) && hookLaunched)
        {
            SwitchHookState();
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
        }
        else if (useMaxDistance)
        {
            //tempGrapplePoint = (Vector2)transform.position + grappleDirection.normalized * maxDistance;
        }
        grapplePoint = tempGrapplePoint;
        springDistance = Vector2.Distance(transform.position, grapplePoint);
        return successfulGrapple;
    }

    private void SwitchHookState()
    {
        // Switch between launched and retracted states
        timeSinceUnhooked = 0;
        hookLaunched = !hookLaunched;
        grapplingHook.transform.position = grapplePoint;
        springJoint.enabled = hookLaunched;
        springJoint.distance = springDistance;
        grapplingHook.transform.parent = hookLaunched ? null : transform;
    }
}
