using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public float moveSpeed = 4f; // Tốc độ di chuyển của nền
    public float increaseInterval = 15f; // Khoảng thời gian tăng tốc độ
    public float speedIncreaseAmount = 1.5f; // Lượng tăng tốc độ mỗi lần
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        // Set thời gian chờ để tăng tốc độ
        timer = increaseInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Di chuyển nền theo trục x
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Giảm thời gian chờ giữa các lần tăng tốc độ
        timer -= Time.deltaTime;

        // Khi thời gian chờ hết, tăng tốc độ và reset timer
        if (timer <= 0)
        {
            moveSpeed += speedIncreaseAmount;
            timer = increaseInterval;
        }
    }

    // Khi nền di chuyển ra khỏi camera, reset vị trí nền
    private void OnBecameInvisible()
    {
        transform.position += new Vector3(30f, 0, 0);
    }
}