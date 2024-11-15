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
        ResizeShape foodResizeShape = food.GetComponent<ResizeShape>();
        if (playerController == null)
        {
            return;
        }

        playerController.TeleportTo(food.transform.position, foodResizeShape);
        foodShape.DestroyShape();
    }

    void HandlePlayerEnemyCollision(GameObject enemy)
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Die();
        }
    }
}
