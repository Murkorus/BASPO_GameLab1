using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

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

    [Header("Audio")]
    [SerializeField] private EventReference hookLaunchSFX;
    [SerializeField] private EventReference hookAttachSFX;
    [SerializeField] private EventReference reelSFX;

    private EventInstance reelInstance;
    private bool reelSoundPlaying = false;

    private SpringJoint2D springJoint;
    private Rigidbody2D rb;

    private Vector2 grapplePoint;
    public float springDistance;
    private float lastSpringDistance;
    private float timeSinceUnhooked;

    [HideInInspector] public bool hookLaunched;
    [HideInInspector] public bool isReeling;
    [HideInInspector] public bool isHookBeingLaunched;
    [HideInInspector] public int reelDirection;
    [HideInInspector] public float hookLaunchDistanceTraveled;
    [HideInInspector] public GameObject grappledObject;
    private float originalDampingRatio;
    private bool doChainBreak;
    private float chainBreakTimeTracker;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        originalDampingRatio = springJoint.dampingRatio;
        lastSpringDistance = springDistance;
    }

    private void Update()
    {
        if (!hookLaunched)
            timeSinceUnhooked += Time.deltaTime;

        if (isHookBeingLaunched)
            ExtendHookChain();

        if (hookLaunched && !doChainBreak)
            doChainBreak = CheckChainBreak();

        if (doChainBreak)
            chainBreakTimeTracker += Time.deltaTime;

        if (doChainBreak && chainBreakTimeTracker > chainBreakTime)
        {
            doChainBreak = false;
            chainBreakTimeTracker = 0;
            SwitchHookState();
        }

        /* ### Inputs ### */

        if (Input.GetMouseButtonDown(0))
        {
            if (!isHookBeingLaunched)
            {
                if (SetGrapplePoint() && timeSinceUnhooked >= hookCooldown)
                {
                    grapplingHook.transform.position = grapplePoint;
                    grapplingHook.transform.parent = hookLaunched ? null : transform;
                    isHookBeingLaunched = true;

                    RuntimeManager.PlayOneShot(hookLaunchSFX, transform.position);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && hookLaunched)
        {
            SwitchHookState();
        }
        else if (Input.GetMouseButtonUp(0) && isHookBeingLaunched)
        {
            hookLaunchDistanceTraveled = 0;
            isHookBeingLaunched = false;
        }

        if (Input.GetMouseButton(1))
        {
            reelDirection = -1;
            isReeling = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isReeling = false;
            reelDirection = 0;
            springJoint.dampingRatio = originalDampingRatio;
        }

        if (isReeling)
        {
            if (reelDirection == -1)
            {
                springDistance = Mathf.Clamp(springDistance - Time.deltaTime * reelingSpeed, 0.05f, distanceWhenHooked);
            }
            else if (reelDirection == 1)
            {
                springDistance = Mathf.Clamp(Vector2.Distance(grapplePoint, transform.position), 0.05f, distanceWhenHooked);
                springJoint.dampingRatio = 0;
            }

            springJoint.distance = springDistance;

            if (springDistance <= 0.05f || springDistance >= distanceWhenHooked)
            {
                reelDirection = 0;
                springJoint.dampingRatio = originalDampingRatio;
            }
        }

        HandleReelSound();
    }

    private void HandleReelSound()
    {
        bool ropeShortening = springDistance < lastSpringDistance - 0.001f;
        lastSpringDistance = springDistance;

        bool isActivelyReeling =
            isReeling &&
            hookLaunched &&
            ropeShortening;

        if (isActivelyReeling && !reelSoundPlaying)
        {
            reelInstance = RuntimeManager.CreateInstance(reelSFX);
            reelInstance.start();
            reelSoundPlaying = true;
        }
        else if (!isActivelyReeling && reelSoundPlaying)
        {
            reelInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            reelInstance.release();
            reelSoundPlaying = false;
        }

        if (reelSoundPlaying)
        {
            float tension = 1f - Mathf.Clamp01(springDistance / distanceWhenHooked);
            reelInstance.setParameterByName("Tension", tension);
        }
    }

    private bool SetGrapplePoint()
    {
        bool successfulGrapple = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 grappleDirection = mousePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, grappleDirection, maxDistance, grappleableLayers);
        if (hit)
        {
            grapplePoint = hit.point;
            successfulGrapple = true;
            grappledObject = hit.collider.gameObject;
        }
        else
        {
            grapplePoint = mousePos;
            grappledObject = null;
        }

        springDistance = Vector2.Distance(transform.position, grapplePoint);
        lastSpringDistance = springDistance;
        return successfulGrapple;
    }

    public void SwitchHookState()
    {
        timeSinceUnhooked = 0;
        distanceWhenHooked = Vector2.Distance(transform.position, grapplePoint);
        hookLaunched = !hookLaunched;
        springJoint.enabled = hookLaunched;
        springJoint.distance = springDistance;
        grapplingHook.transform.position = grapplePoint;
        grapplingHook.transform.parent = hookLaunched ? null : transform;

        if (hookLaunched)
            RuntimeManager.PlayOneShot(hookAttachSFX, grapplePoint);
    }

    private bool CheckChainBreak()
    {
        Vector2 chainVector = (Vector2)transform.position - grapplePoint;
        chainVector.Normalize();
        float dotProduct = rb.linearVelocity.x * chainVector.x + rb.linearVelocity.y * chainVector.y;
        return dotProduct > chainBreakForce;
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
        else if (hookLaunchDistanceTraveled > maxDistance)
        {
            isHookBeingLaunched = false;
            hookLaunchDistanceTraveled = 0;
        }
    }
}
