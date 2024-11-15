// ShapeCollisionHandler.cs
using UnityEngine;

public class ShapeCollisionHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("Player"))
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
        PlayerController playerController = GetComponent<PlayerController>();
        DrawShape foodShape = food.GetComponent<DrawShape>();
        if (playerController == null)
        {
            return;
        }

        playerController.TeleportTo(food.transform.position);
        foodShape.DestroyShape();
    }

    void HandlePlayerEnemyCollision(GameObject enemy)
    {
        Debug.Log("Player Collided with enemy: " + enemy.name);
        // Add additional enemy collision handling here
    }
}
