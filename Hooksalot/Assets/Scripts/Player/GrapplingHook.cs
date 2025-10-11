using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] Transform grapplePivot;
    [SerializeField] LayerMask grappleableLayers;

    [SerializeField] bool useMaxDistance;
    [SerializeField] float maxDistance;
    [SerializeField] float reelingSpeed;

    [Header("References")]
    [SerializeField] GameObject grapplingHook;
    private SpringJoint2D springJoint;
    

    private Vector2 grapplePoint;
    [HideInInspector] public bool hookLaunched;
    [HideInInspector] public bool isReeling;
    public float springDistance;

    private void Start()
    {
        springJoint = GetComponent<SpringJoint2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetGrapplePoint();
            SwitchHookState();
        }
        if (Input.GetMouseButtonUp(0))
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

    private void SetGrapplePoint()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 grappleDirection = mousePos - (Vector2)grapplePivot.position;
        Vector2 tempGrapplePoint = mousePos;
        RaycastHit2D hit = Physics2D.Raycast(grapplePivot.position, grappleDirection, useMaxDistance ? maxDistance : Mathf.Infinity, grappleableLayers);
        if (hit)
        {
            tempGrapplePoint = hit.point;
        }
        else if (useMaxDistance)
        {
            //tempGrapplePoint = (Vector2)transform.position + grappleDirection.normalized * maxDistance;
        }
        grapplePoint = tempGrapplePoint;
        springDistance = Vector2.Distance(transform.position, grapplePoint);
    }

    private void SwitchHookState()
    {
        // Switch between launched and retracted states
        hookLaunched = !hookLaunched;
        grapplingHook.transform.position = grapplePoint;
        springJoint.enabled = hookLaunched;
        springJoint.distance = springDistance;
        grapplingHook.transform.parent = hookLaunched ? null : transform;
    }
}
