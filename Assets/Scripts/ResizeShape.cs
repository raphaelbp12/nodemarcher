using UnityEngine;

[RequireComponent(typeof(DrawShape))]
public class ResizeShape : MonoBehaviour
{
    DrawShape drawShape;
    float desiredScale = 1.0f;
    float currentScale = 1.0f;
    float expandSpeed = 2f;
    float shrinkSpeed = 2f;

    float originalOuterRadius;
    float originalInnerRadius;
    void Start()
    {
        drawShape = GetComponent<DrawShape>();
        originalOuterRadius = drawShape.polygonOuterRadius;
        originalInnerRadius = drawShape.polygonInnerRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScale < desiredScale)
        {
            currentScale += expandSpeed * Time.deltaTime;
            if (currentScale > desiredScale)
            {
                currentScale = desiredScale;
            }
        }
        else if (currentScale > desiredScale)
        {
            currentScale -= shrinkSpeed * Time.deltaTime;
            if (currentScale < desiredScale)
            {
                currentScale = desiredScale;
            }
        }
        // transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        drawShape.SetPolygon(drawShape.polygonSides, originalOuterRadius * currentScale, originalInnerRadius * currentScale, drawShape.isFilled);      
    }

    public void SetScale(float scale)
    {
        desiredScale = scale;
    }
}
