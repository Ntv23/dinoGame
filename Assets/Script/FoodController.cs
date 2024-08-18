using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của food
    private Rigidbody2D rbFood;  // Rigidbody của food

    void Start()
    {
        rbFood = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Di chuyển food theo tốc độ của ground
        Vector2 velocity = new Vector2(-moveSpeed, rbFood.velocity.y);
        rbFood.velocity = velocity;

        // Kiểm tra nếu food ra khỏi camera thì destroy
        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
        }
    }
}
