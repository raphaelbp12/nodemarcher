using UnityEngine;

[RequireComponent(typeof(DrawShape))]
public class ResizeShape : MonoBehaviour
{
    DrawShape drawShape;
    [SerializeField]
    float defaultScale = 0.1f;
    float currentScale;
    float desiredScale;
    [SerializeField]
    float expandSpeed = 2f;
    [SerializeField]
    float shrinkSpeed = 2f;

    public float originalOuterRadius { get; private set; }
    float originalInnerRadius;
    void Start()
    {
        drawShape = GetComponent<DrawShape>();
        originalOuterRadius = drawShape.polygonOuterRadius;
        originalInnerRadius = drawShape.polygonInnerRadius;

        currentScale = defaultScale;
        desiredScale = defaultScale;
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
        drawShape.SetPolygon(drawShape.polygonSides, originalOuterRadius * currentScale, originalInnerRadius * currentScale, drawShape.isFilled, drawShape.entityType);
    }

    public void OverrideShape(float scale)
    {
        currentScale = scale;
        drawShape.SetPolygon(drawShape.polygonSides, originalOuterRadius * currentScale, originalInnerRadius * currentScale, drawShape.isFilled, drawShape.entityType);
    }

    public void SetScale(float? scale)
    {
        if (scale == null)
        {
            desiredScale = defaultScale;
            return;
        }
        desiredScale = scale.Value;
    }
}
