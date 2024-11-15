using UnityEngine;

public class RotateShape : MonoBehaviour
{
    public float rotationBaseSpeed = 1.0f;
    public float rotationRangePercentage = 0.5f;
    private float rotationSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationSpeed = rotationBaseSpeed * Random.Range(0.2f * rotationRangePercentage, rotationRangePercentage);
        rotationSpeed *= Random.value > 0.5f ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);        
    }
}
