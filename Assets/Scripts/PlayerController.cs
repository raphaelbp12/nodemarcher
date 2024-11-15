using UnityEngine;

[RequireComponent(typeof(ResizeShape))]
public class PlayerController : MonoBehaviour
{
    ResizeShape resizeShape;
    private bool isKeyBeingHeld = false;
    private float maxScale = 4.0f;

    void Start()
    {
        resizeShape = GetComponent<ResizeShape>();
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
