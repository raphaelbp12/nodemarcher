using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    List<DrawShape> shapes = new List<DrawShape>();

    public float polygonOuterRadius = 0.5f;
    public float polygonInnerRadius = 0.3f;
    public float gridWidth = 50;
    public float gridHeight = 50;

    public int lastTick = 0;
    public int tickRate = 10000;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (lastTick == 0 || Time.frameCount - lastTick > tickRate)
        {
            lastTick = Time.frameCount;
            List<Vector3> positions = GetGridPositions(0, 0, gridWidth, gridHeight);
            foreach (Vector3 position in positions)
            {
                DrawShape shape = SpawnEnemy(position, 0.1f, 0.5f, 6);
                shapes.Add(shape);
            }
        }
    }

    DrawShape SpawnEnemy(Vector3 position, float thickness, float maxRadius, int sides)
    {
        float randomFactor = Random.Range(0.6f, 1.0f);
        float randomRadius = maxRadius * randomFactor;

        float diameterDifference = (maxRadius - randomRadius) * 2;
        float randomX = position.x + Random.Range(-diameterDifference, diameterDifference);
        float randomY = position.y + Random.Range(-diameterDifference, diameterDifference);

        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        DrawShape shape = enemy.GetComponent<DrawShape>();
        shape.isFilled = false;
        shape.polygonSides = sides;
        shape.polygonOuterRadius = randomRadius;
        shape.polygonInnerRadius = randomRadius - thickness;
        
        return shape;
    }

    List<Vector3> GetGridPositions(float x, float y, float width, float height)
    {
        List<Vector3> positions = new List<Vector3>();
        float minWidth = x - width / 2.0f;
        float minHeight = y - height / 2.0f;

        for (float i = minWidth; i < minWidth + width; i++)
        {
            for (float j = minHeight; j < minHeight + height; j++)
            {
                if (i == x && j == y)
                {
                    continue;
                }
                positions.Add(new Vector3(i, j, 0));
            }
        }

        return positions;
    }
}
