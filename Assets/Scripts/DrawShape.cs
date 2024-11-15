// DrawShape.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class DrawShape : MonoBehaviour
{
    Mesh mesh;
    Vector3[] polygonPoints;
    int[] polygonTriangles;

    public bool isFilled { get; private set; }
    public int polygonSides { get; private set; }

    public float polygonOuterRadius { get; private set; }
    public float polygonInnerRadius { get; private set; }

    // Grid position where this shape was spawned
    public Vector3 gridPosition { get; private set; }

    // Reference to the GameManager
    private GameManager gameManager;

    [SerializeField]
    private float distanceToBeDisabled = 5f; // Adjust as needed

    PlayerController playerController;
    ResizeShape resizeShape;

    public EntityType entityType { get; private set; }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        resizeShape = GetComponent<ResizeShape>();
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            playerController = playerGO.GetComponent<PlayerController>();
        }
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFilled)
        {
            DrawFilled(polygonSides, polygonOuterRadius);
        }
        else
        {
            DrawHollow(polygonSides, polygonOuterRadius, polygonInnerRadius);
        }
        SetPolygonCollider();
        SetDefaultScale();
    }

    void FixedUpdate()
    {
        DisableIfTooFar();
    }

    void SetDefaultScale()
    {
        var selfPlayer = GetComponent<PlayerController>();
        if (selfPlayer != null)
        {
            return;
        }
        resizeShape.SetScale(1.0f);
    }

    void DisableIfTooFar()
    {
        if (GetDistanceToPlayer() > distanceToBeDisabled)
        {
            DestroyShape();
        }
    }

    public float GetDistanceToPlayer()
    {
        if (entityType == EntityType.Player)
        {
            return 0;
        }
        if (playerController == null)
            return Mathf.Infinity;
        return Vector3.Distance(playerController.transform.position, transform.position);
    }

    public void DestroyShape()
    {
        if (gameManager != null)
        {
            gameManager.RemoveSpawnedPosition(gridPosition, entityType);
        }
        Destroy(gameObject);
    }

    public void SetPolygon(int sides, float outerRadius, float innerRadius, bool filled, EntityType newEntityType)
    {
        polygonSides = sides;
        polygonOuterRadius = outerRadius;
        polygonInnerRadius = innerRadius;
        isFilled = filled;
        entityType = newEntityType;
        SetPolygonCollider();
    }

    public void SetGridPosition(Vector3 position)
    {
        gridPosition = position;
    }

    void SetPolygonCollider()
    {
        if (polygonPoints == null || polygonPoints.Length == 0)
        {
            return;
        }
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.pathCount = 1;
        Vector2[] polygonPoints2D = Array.ConvertAll(polygonPoints, v => new Vector2(v.x, v.y));
        polygonCollider.SetPath(0, polygonPoints2D);
        polygonCollider.isTrigger = true;
    }

    void DrawHollow(int sides, float outerRadius, float innerRadius)
    {
        List<Vector3> pointsList = new List<Vector3>();
        List<Vector3> outerPoints = GetCircumferencePoints(sides, outerRadius);
        pointsList.AddRange(outerPoints);
        List<Vector3> innerPoints = GetCircumferencePoints(sides, innerRadius);
        pointsList.AddRange(innerPoints);

        polygonPoints = pointsList.ToArray();

        polygonTriangles = DrawHollowTriangles(polygonPoints);
        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
    }

    int[] DrawHollowTriangles(Vector3[] points)
    {
        int sides = points.Length / 2;

        List<int> newTriangles = new List<int>();
        for (int i = 0; i < sides; i++)
        {
            int outerIndex = i;
            int innerIndex = i + sides;

            newTriangles.Add(outerIndex);
            newTriangles.Add(innerIndex);
            newTriangles.Add((i + 1) % sides);

            newTriangles.Add(outerIndex);
            newTriangles.Add(sides + ((sides + i - 1) % sides));
            newTriangles.Add(outerIndex + sides);
        }
        return newTriangles.ToArray();
    }

    void DrawFilled(int sides, float radius)
    {
        polygonPoints = GetCircumferencePoints(sides, radius).ToArray();
        polygonTriangles = DrawFilledTriangles(polygonPoints);
        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
    }

    private int[] DrawFilledTriangles(Vector3[] polygonPoints)
    {
        int triangleAmount = polygonPoints.Length - 2;
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangleAmount; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i + 1);
            newTriangles.Add(i + 2);
        }
        return newTriangles.ToArray();
    }

    private List<Vector3> GetCircumferencePoints(int sides, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        float circumferenceProgressPerStep = (float)1 / sides;
        float TAU = Mathf.PI * 2;
        float radianProgressPerStep = TAU * circumferenceProgressPerStep;

        for (int i = 0; i < sides; i++)
        {
            float currentRadian = radianProgressPerStep * i;
            points.Add(new Vector3(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius, 0));
        }
        return points;
    }
}
