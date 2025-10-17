using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    // This script is attached to a GameObject which is placed on the same spot as the grapple point from the GrapplingHook script.
    [SerializeField] GrapplingHook hookScript;
    private LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (hookScript.hookLaunched)
        {
            lineRenderer.SetPositions(new Vector3[]
            {
                transform.position,
                hookScript.transform.position
            });
        }
        if(hookScript.isHookBeingLaunched)
        {
            lineRenderer.SetPositions(new Vector3[]
            {
                Vector2.Lerp(hookScript.transform.position, transform.position, hookScript.hookLaunchDistanceTraveled / Vector2.Distance(transform.position, hookScript.transform.position)),
                hookScript.transform.position
            });
        }
        lineRenderer.enabled = hookScript.hookLaunched || hookScript.isHookBeingLaunched;
        
    }
}
