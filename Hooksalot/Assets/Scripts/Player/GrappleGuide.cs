using UnityEngine;

public class GrappleGuide : MonoBehaviour
{
    // This script is attached to a gameobject which is a child of the player.
    // It will have a line renderer that shows a guideline to let the player aim more easily.

    Vector2 lineStart;
    Vector2 lineEnd;

    LineRenderer guideLine;
    SpriteRenderer targetSprite;
    public Transform grappleRope;

    private void Start()
    {
        guideLine = GetComponent<LineRenderer>();
        targetSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!GameManager.hook.hookLaunched)
        {
            if (GameManager.hook.isHookBeingLaunched)
            {
                lineStart = grappleRope.position;
            }
            else
            {
                Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - GameManager.hook.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(GameManager.hook.transform.position, lookDirection, GameManager.hook.maxDistance, GameManager.hook.grappleableLayers);
                Vector2 hitPoint;
                if (hit)
                {
                    hitPoint = hit.point;
                }
                else
                {
                    hitPoint = (Vector2)GameManager.hook.transform.position + lookDirection.normalized * GameManager.hook.maxDistance;
                }
                lineStart = hitPoint;
            }

            lineEnd = GameManager.hook.transform.position;
            if (GameManager.hook.isHookBeingLaunched)
            {
                lineEnd = Vector2.Lerp(GameManager.hook.transform.position, lineStart, GameManager.hook.hookLaunchDistanceTraveled / Vector2.Distance(GameManager.hook.transform.position, lineStart));
            }

            guideLine.SetPositions(new Vector3[]
            {
                GameManager.hook.isHookBeingLaunched ? lineStart : lineEnd,
                GameManager.hook.isHookBeingLaunched ? lineEnd : lineStart
            });

            transform.position = lineStart;
        }
        guideLine.enabled = !GameManager.hook.hookLaunched;
        targetSprite.enabled = !GameManager.hook.hookLaunched;
    }
}
