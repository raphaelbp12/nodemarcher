using UnityEngine;

public class ShapeCollisionHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (other.CompareTag("Food"))
            {
                HandlePlayerFoodCollision(other.gameObject);
            }
            else if (other.CompareTag("Enemy"))
            {
                HandlePlayerEnemyCollision(other.gameObject);
            }
        }
    }

    void HandlePlayerFoodCollision(GameObject food)
    {
        Debug.Log("Player Collided with food" + food.gameObject.name);
    }

    void HandlePlayerEnemyCollision(GameObject enemy)
    {
        Debug.Log("Player Collided with enemy" + enemy.gameObject.name);
    }
}
