using UnityEngine;


[RequireComponent(typeof(DrawShape))]
[RequireComponent(typeof(ResizeShape))]
public class PlayerController : MonoBehaviour
{
    Camera mainCamera;
    DrawShape drawShape;
    ResizeShape resizeShape;
    private bool isKeyBeingHeld = false;
    private float maxScale = 4.0f;
    private float originalScale;

    void Start()
    {
        resizeShape = GetComponent<ResizeShape>();
        drawShape = GetComponent<DrawShape>();
        drawShape.SetPolygon(12, 0.5f, 0.4f, false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        if (Input.anyKeyDown || Input.anyKey)
        {
            isKeyBeingHeld = true;
        }
        else
        {
            isKeyBeingHeld = false;
        }

        if (isKeyBeingHeld)
        {
            resizeShape.SetScale(maxScale);
        }
        else
        {
            resizeShape.SetScale(null);
        }
    }

    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }

    void UpdateCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        }
    }
}
