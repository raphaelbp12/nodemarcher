using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PaintShape : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public Color color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();    
        meshRenderer.material.color = color;    
    }

    void SetColor(Color newColor)
    {
        color = newColor;
        meshRenderer.material.color = color;
    }
}
