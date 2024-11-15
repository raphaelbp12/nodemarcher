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
        PlayerController playerController = gameObject.GetComponent<PlayerController>();
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
        Debug.Log("Player Collided with enemy" + enemy.gameObject.name);
    }
}
