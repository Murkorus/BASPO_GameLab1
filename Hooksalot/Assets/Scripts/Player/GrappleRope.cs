using UnityEngine;

public class GrappleRope : MonoBehaviour
{
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
        lineRenderer.enabled = hookScript.hookLaunched;
        
    }
}
