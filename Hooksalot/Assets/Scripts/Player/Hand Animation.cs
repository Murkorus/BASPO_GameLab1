using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    public float maxHandDistance;
    public Transform handTransform;
    
    public void Update()
    {
        Vector2 handAnimation;
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - GameManager.hook.transform.position;
        handAnimation = handTransform.position = lookDirection.normalized;
        handTransform.localPosition = handAnimation;
    }
    
}
