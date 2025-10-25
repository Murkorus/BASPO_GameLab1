using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // This script is attached to a GameObject which is the parent of a bunch of SpriteRenderers, and a child of the Main Camera.
    // Each child will be a collection of sprites in a collumn, such that the total height of the sprites in each child is 3 times teh combined height of the viewport.
    // This script will make each child move at a slightly different speed, such that the illusion of parallax is created.
    // When the viewport would move past a sprite, the child is teleported up or down such that it's always in frame.

    [SerializeField] SpriteRenderer[] backgroundSprites = new SpriteRenderer[0];
    [SerializeField] float[] parallaxStrength = new float[0]; // 0 means the picture stays in place, 1 means the picture moves with the camera.
    private float[] centerPositions;
    private float camHeight;

    private void Start()
    {
        centerPositions = new float[backgroundSprites.Length];
        for (int i = 0; i < centerPositions.Length; i++)
        {
            centerPositions[i] = backgroundSprites[i].transform.position.y;
        }
        camHeight = GameManager.halfScreenSize.y * 2;
    }

    private void Update()
    {
        for(int i = 0; i < backgroundSprites.Length; i++)
        {
            Transform spriteTransform = backgroundSprites[i].transform;
            float cameraRelativePosition = Camera.main.transform.position.y * (1 - parallaxStrength[i]);
            float cameraDistanceTraveled = Camera.main.transform.position.y * parallaxStrength[i];

            spriteTransform.position = new Vector2(spriteTransform.position.x, centerPositions[i] + cameraDistanceTraveled);

            if(cameraRelativePosition > centerPositions[i] + camHeight)
            {
                centerPositions[i] += camHeight;
            }
            else if(cameraRelativePosition < centerPositions[i] - camHeight)
            {
                centerPositions[i] -= camHeight;
            }
        }
    }
}
