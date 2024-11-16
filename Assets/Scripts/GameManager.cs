// GameManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;

    [Header("References")]
    public GameObject playerPrefab;
    public GameObject foodPrefab;
    public GameObject enemyPrefab;
    private GameObject playerGameObject;


    [Header("Food Settings")]
    [SerializeField]
    private float polygonOuterRadius = 0.5f;
    [SerializeField]
    private float polygonThickness = 0.1f;
    [SerializeField]
    private int gridWidth = 10;
    [SerializeField]
    private int gridHeight = 10;

    [Header("Spawning Settings")]
    [SerializeField]
    private int tickRate = 60; // Number of frames between spawns
    [SerializeField]
    private int minSpawnDistance = 2; // Minimum distance from the player to spawn food

    [Header("Enemy Spawning Settings")]
    [SerializeField]
    private int enemyMinDistanceFromPlayer = 3; // Minimum distance from player to spawn enemy
    [SerializeField]
    private int enemyMinDistanceFromEnemies = 2; // Minimum distance between enemies
    [SerializeField]
    private int enemySpawnCountPerTick = 2; // Maximum number of enemies to spawn per tick

    private int lastTick = 0;
    private bool hasInitialSpawnOccurred = false;

    // Using HashSet for O(1) lookup and removal
    private HashSet<Vector2Int> spawnedFoodPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> spawnedEnemyPositions = new HashSet<Vector2Int>();

    void Start()
    {
        // Validate References
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the GameManager.");
        }

        if (foodPrefab == null)
        {
            Debug.LogError("Food Prefab is not assigned in the GameManager.");
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the GameManager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (Time.frameCount - lastTick > tickRate)
        {
            lastTick = Time.frameCount;
            SpawnEntitiesAroundPlayer();
        }
    }

    void SpawnPlayer(bool checkKey)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the GameManager.");
            return;
        }

        if (playerGameObject != null)
        {
            Destroy(playerGameObject);
        }

        playerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var drawShape = playerGameObject.GetComponent<DrawShape>();
        drawShape.SetPolygon(12, 0.5f, 0.4f, false, EntityType.Player);
        var playerController = playerGameObject.GetComponent<PlayerController>();
        playerController.SetGetKey(checkKey);
    }

    /// <summary>
    /// Spawns enemies and food around the player's current position based on defined conditions.
    /// </summary>
    void SpawnEntitiesAroundPlayer()
    {
        if (playerGameObject == null || foodPrefab == null || enemyPrefab == null)
        {
            return; // Early exit if references are missing
        }

        Vector3 playerPos = playerGameObject.transform.position;
        Vector2Int playerGridPos = new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));

        List<Vector3> gridPositions = GetGridPositions(playerGridPos.x, playerGridPos.y, gridWidth, gridHeight, hasInitialSpawnOccurred);

        // Shuffle gridPositions for random spawn order (optional but recommended)
        ShuffleList(gridPositions);

        int enemiesSpawnedThisTick = 0;

        foreach (Vector3 position in gridPositions)
        {
            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

            // **Enemy Spawn Logic:**
            if (enemiesSpawnedThisTick < enemySpawnCountPerTick &&
                CanSpawnEnemyAtPosition(gridPos, playerGridPos))
            {
                SpawnEntity(enemyPrefab, gridPos, position, EntityType.Enemy, polygonOuterRadius, 6, polygonThickness);
                enemiesSpawnedThisTick++;
            }
            else
            {
                // **Food Spawn Logic:**
                SpawnEntity(foodPrefab, gridPos, position, EntityType.Food, polygonOuterRadius, 6, polygonThickness);
            }
        }

        // After the first spawn, set the flag to true
        if (!hasInitialSpawnOccurred)
        {
            hasInitialSpawnOccurred = true;
        }
    }

    /// <summary>
    /// Determines if an enemy can be spawned at the given grid position.
    /// </summary>
    /// <param name="gridPos">Grid position to check.</param>
    /// <param name="playerGridPos">Player's grid position.</param>
    /// <returns>True if an enemy can be spawned; otherwise, false.</returns>
    bool CanSpawnEnemyAtPosition(Vector2Int gridPos, Vector2Int playerGridPos)
    {
        // Check distance from player
        float distanceFromPlayer = Vector2Int.Distance(gridPos, playerGridPos);
        if (distanceFromPlayer < enemyMinDistanceFromPlayer)
        {
            return false;
        }

        // Check distance from other enemies
        foreach (Vector2Int enemyPos in spawnedEnemyPositions)
        {
            float distanceFromEnemy = Vector2Int.Distance(gridPos, enemyPos);
            if (distanceFromEnemy < enemyMinDistanceFromEnemies)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Spawns an entity (Food or Enemy) at the specified position and grid position.
    /// </summary>
    /// <param name="prefab">Prefab to spawn.</param>
    /// <param name="gridPos">Grid position of the spawn.</param>
    /// <param name="worldPos">World position to spawn the entity.</param>
    /// <param name="entityType">Type of entity to spawn.</param>
    void SpawnEntity(GameObject prefab, Vector2Int gridPos, Vector3 worldPos, EntityType entityType, float maxRadius, int sides, float thickness)
    {
        // Check if the position is already occupied
        if (entityType == EntityType.Food && (spawnedFoodPositions.Contains(gridPos) || spawnedEnemyPositions.Contains(gridPos)))
        {
            return; // Already occupied, skip
        }
        if (entityType == EntityType.Enemy && (spawnedFoodPositions.Contains(gridPos) || spawnedEnemyPositions.Contains(gridPos)))
        {
            return; // Already occupied, skip
        }

        float randomFactor = Random.Range(0.4f, 0.8f);
        float randomRadius = maxRadius * randomFactor;

        // Calculate random offset within the grid cell to avoid overlapping exactly on the grid point
        float diameterDifference = (polygonOuterRadius - polygonThickness) * 2 * 0.5f;
        float randomX = worldPos.x + Random.Range(-diameterDifference, diameterDifference);
        float randomY = worldPos.y + Random.Range(-diameterDifference, diameterDifference);

        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        GameObject entity = Instantiate(prefab, randomPosition, Quaternion.identity);
        DrawShape shape = entity.GetComponent<DrawShape>();
        shape.SetPolygon(sides, randomRadius, randomRadius - thickness, false, entityType);
        shape.SetGridPosition(new Vector3(gridPos.x, gridPos.y, 0));

        if (entityType == EntityType.Food)
        {
            spawnedFoodPositions.Add(gridPos);
        }
        else if (entityType == EntityType.Enemy)
        {
            spawnedEnemyPositions.Add(gridPos);
        }
    }

    /// <summary>
    /// Retrieves a list of grid positions around the player where entities can be spawned.
    /// Excludes positions within the minimum spawn distance.
    /// </summary>
    /// <param name="x">Player's grid X position.</param>
    /// <param name="y">Player's grid Y position.</param>
    /// <param name="width">Width of the grid area.</param>
    /// <param name="height">Height of the grid area.</param>
    /// <param name="hasInitialSpawnOccurred">Flag indicating if initial spawn has occurred.</param>
    /// <returns>List of eligible grid positions for spawning entities.</returns>
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

                // During initial spawn, allow entities close to the player
                if (hasInitialSpawnOccurred && distance < minSpawnDistance)
                {
                    continue; // Skip positions too close
                }

                Vector2Int gridPos = new Vector2Int(i, j);
                if (!spawnedFoodPositions.Contains(gridPos) && !spawnedEnemyPositions.Contains(gridPos))
                {
                    positions.Add(new Vector3(i, j, 0));
                }
            }
        }

        return positions;
    }
    public void RemoveSpawnedPosition(Vector3 position, EntityType entityType)
    {
        if (entityType == EntityType.Food)
        {
            RemoveFoodPosition(position);
        }
        else if (entityType == EntityType.Enemy)
        {
            RemoveEnemyPosition(position);
        }
    }

    /// <summary>
    /// Removes the specified grid position from spawnedFoodPositions.
    /// Called by Food when it's destroyed due to being too far from the player.
    /// </summary>
    /// <param name="position">The grid position to remove.</param>
    public void RemoveFoodPosition(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        if (spawnedFoodPositions.Contains(gridPos))
        {
            spawnedFoodPositions.Remove(gridPos);
        }
        else
        {
            Debug.LogWarning($"Attempted to remove a food grid position that wasn't spawned: {gridPos}");
        }
    }

    /// <summary>
    /// Removes the specified grid position from spawnedEnemyPositions.
    /// Called by Enemy when it's destroyed due to being too far from the player.
    /// </summary>
    /// <param name="position">The grid position to remove.</param>
    public void RemoveEnemyPosition(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        if (spawnedEnemyPositions.Contains(gridPos))
        {
            spawnedEnemyPositions.Remove(gridPos);
        }
        else
        {
            Debug.LogWarning($"Attempted to remove an enemy grid position that wasn't spawned: {gridPos}");
        }
    }

    private void HandleInput()
    {
        if (Input.anyKeyDown || Input.anyKey)
        {
            if (gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(false);
                if (playerGameObject == null)
                {
                    ResetGame();
                    SpawnPlayer(true);
                }
            }
        }
    }

    /// <summary>
    /// Handles player death.
    /// </summary>
    public void PlayerDied()
    {
        gameOverPanel.SetActive(true);
        ResetGame();
        SpawnPlayer(false);
        Debug.Log("Player has died.");
        // Implement sound and particle effects here when implemented
        // Example:
        // AudioSource.PlayClipAtPoint(deathSound, transform.position);
        // Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    private void ResetGame()
    {
        if (playerGameObject != null)
        {
            Destroy(playerGameObject);
        }
        spawnedFoodPositions.Clear();
        spawnedEnemyPositions.Clear();
        hasInitialSpawnOccurred = false;
        lastTick = 0;
    }

    /// <summary>
    /// Shuffles a list in place using Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">Type of list elements.</typeparam>
    /// <param name="list">List to shuffle.</param>
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}

/// <summary>
/// Enumeration for entity types.
/// </summary>
public enum EntityType
{
    Food,
    Enemy,
    Player
}
