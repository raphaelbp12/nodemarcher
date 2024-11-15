using UnityEngine;


[RequireComponent(typeof(DrawShape))]
[RequireComponent(typeof(ResizeShape))]
public class PlayerController : MonoBehaviour
{
    DrawShape drawShape;
    ResizeShape resizeShape;
    private bool isKeyBeingHeld = false;
    private float maxScale = 4.0f;

    void Start()
    {
        resizeShape = GetComponent<ResizeShape>();
        drawShape = GetComponent<DrawShape>();
        drawShape.SetPolygon(12, 0.5f, 0.4f, false);
    }

    // Update is called once per frame
    void Update()
    {
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
            resizeShape.SetScale(1.0f);
        }
    }
}
