using System.Collections.Generic;
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
    private int tickRate = 60; // Assuming tickRate is in frames
    [SerializeField]
    private int minSpawnDistance = 2; // Minimum distance from the player to spawn food

    // Using HashSet for O(1) lookup and removal
    private HashSet<Vector2Int> spawnedPositions = new HashSet<Vector2Int>();


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var hasInitialSpawnOccurred = lastTick != 0;
        if (lastTick == 0 || Time.frameCount - lastTick > tickRate)
        {
            var playerPos = playerGO.transform.position;
            List<Vector3> positions = GetGridPositions(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y), gridWidth, gridHeight, hasInitialSpawnOccurred);
            foreach (Vector3 position in positions)
            {
                SpawnFood(position, polygonThickness, polygonOuterRadius, 6);
            }
            lastTick = Time.frameCount;
        }
    }

    DrawShape SpawnFood(Vector3 position, float thickness, float maxRadius, int sides)
    {
        Vector2Int positionInt = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        // Check if the position is already spawned
        if (spawnedPositions.Contains(positionInt))
        {
            return null; // Already spawned, skip
        }

        float randomFactor = Random.Range(0.4f, 0.8f);
        float randomRadius = maxRadius * randomFactor;

        float diameterDifference = (maxRadius - randomRadius) * 2 * 0.5f;
        float randomX = position.x + Random.Range(-diameterDifference, diameterDifference);
        float randomY = position.y + Random.Range(-diameterDifference, diameterDifference);

        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
        DrawShape shape = food.GetComponent<DrawShape>();
        shape.SetPolygon(sides, randomRadius, randomRadius - thickness, false);

        // Assign the grid position to the shape
        shape.SetGridPosition(position);

        // Add the grid position to spawnedPositions
        spawnedPositions.Add(positionInt);

        return shape;
    }

    List<Vector3> GetGridPositions(int x, int y, int width, int height, bool hasInitialSpawnOccurred)
    {
        List<Vector3> positions = new List<Vector3>();
        int minWidth = x - width / 2;
        int minHeight = y - height / 2;

        for (int i = minWidth; i < minWidth + width; i++)
        {
            for (int j = minHeight; j < minHeight + height; j++)
            {
                // Skip the player's current position
                if (i == x && j == y)
                {
                    continue;
                }

                // Calculate the Manhattan distance from the player
                int distance = Mathf.Abs(i - x) + Mathf.Abs(j - y);

                // During initial spawn, allow food close to the player
                if (hasInitialSpawnOccurred && distance < minSpawnDistance)
                {
                    continue;
                }

                Vector3 position = new Vector3(i, j, 0);
                Vector2Int positionInt = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
                if (!spawnedPositions.Contains(positionInt))
                {
                    positions.Add(position);
                }
            }
        }

        return positions;
    }

    /// <summary>
    /// Removes the specified grid position from spawnedPositions.
    /// Called by DrawShape when it's destroyed due to being too far from the player.
    /// </summary>
    /// <param name="position">The grid position to remove.</param>
    public void RemoveSpawnedPosition(Vector3 position)
    {
        Vector2Int positionInt = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        if (spawnedPositions.Contains(positionInt))
        {
            spawnedPositions.Remove(positionInt);
            Debug.Log($"Removed grid position: {position}");
        }
        else
        {
            Debug.LogWarning($"Attempted to remove a grid position that wasn't spawned: {position}");
        }
    }
}
