using UnityEngine;


[RequireComponent(typeof(Camera))]
public class ScreenAdjust : MonoBehaviour
{
    // 9:16 portrait aspect ratio
    private const float TargetAspect = 9f / 16f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        AdjustViewport();
    }

    private void Update()
    {
        AdjustViewport();
    }

    private void AdjustViewport()
    {
        float screenAspect =
            (float)Screen.width / Screen.height;

        // We always use the FULL height of the screen.
        float viewportWidth =
            TargetAspect / screenAspect; 

        // Center the portrait viewport horizontally.
        float viewportX =
            (1f - viewportWidth) / 2f;

        cam.rect = new Rect(
            viewportX,
            0f,
            viewportWidth,
            1f
        );
    }
}
