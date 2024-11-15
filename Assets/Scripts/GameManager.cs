using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerGO;
    public GameObject foodPrefab;
    private float polygonOuterRadius = 0.5f;
    private float polygonThickness = 0.1f;
    private int gridWidth = 10;
    private int gridHeight = 10;

    private int lastTick = 0;
    [SerializeField]
    private int tickRate = 1;
    HashSet<Vector3> spawnedPositions = new HashSet<Vector3>();

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (lastTick == 0 || Time.frameCount - lastTick > tickRate)
        {
            lastTick = Time.frameCount;
            var playerPos = playerGO.transform.position;
            List<Vector3> positions = GetGridPositions(Mathf.CeilToInt(playerPos.x), Mathf.CeilToInt(playerPos.y), gridWidth, gridHeight);
            foreach (Vector3 position in positions)
            {
                SpawnFood(position, polygonThickness, polygonOuterRadius, 6);
            }
        }
    }

    DrawShape SpawnFood(Vector3 position, float thickness, float maxRadius, int sides)
    {
        float randomFactor = Random.Range(0.4f, 0.8f);
        float randomRadius = maxRadius * randomFactor;

        float diameterDifference = (maxRadius - randomRadius) * 2 * 0.5f;
        float randomX = position.x + Random.Range(-diameterDifference, diameterDifference);
        float randomY = position.y + Random.Range(-diameterDifference, diameterDifference);

        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
        DrawShape shape = food.GetComponent<DrawShape>();
        shape.SetPolygon(sides, randomRadius, randomRadius - thickness, false);
        
        return shape;
    }

    List<Vector3> GetGridPositions(int x, int y, int width, int height)
    {
        List<Vector3> positions = new List<Vector3>();
        int minWidth = x - width / 2;
        int minHeight = y - height / 2;

        for (int i = minWidth; i < minWidth + width; i++)
        {
            for (int j = minHeight; j < minHeight + height; j++)
            {
                if (i == x && j == y)
                {
                    continue;
                }
                Vector3 position = new Vector3(i, j, 0);
                if (!spawnedPositions.Contains(position))
                {
                    positions.Add(position);
                    spawnedPositions.Add(position);
                }
            }
        }

        return positions;
    }
}
