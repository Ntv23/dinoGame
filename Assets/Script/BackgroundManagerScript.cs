using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManagerScript : MonoBehaviour
{
    public GameObject backgroundPrefab; // Prefab của nền
    public float moveSpeed = 4f; // Tốc độ di chuyển của nền
    public float increaseInterval = 15f; // Khoảng thời gian tăng tốc độ
    public float speedIncreaseAmount = 1.5f; // Lượng tăng tốc độ mỗi lần
    public int initialBackgroundCount = 3; // Số lượng nền ban đầu
    private List<GameObject> backgrounds = new List<GameObject>(); // Danh sách các nền
    private float timer;

    void Start()
    {
        // Spawn nền ban đầu
        for (int i = 0; i < initialBackgroundCount; i++)
        {
            GameObject bg = Instantiate(backgroundPrefab, new Vector3(i * backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x, 0, 0), Quaternion.identity);
            backgrounds.Add(bg);
        }

        // Set thời gian chờ để tăng tốc độ
        timer = increaseInterval;
    }

    void Update()
    {
        // Di chuyển tất cả các nền
        foreach (GameObject bg in backgrounds)
        {
            bg.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // Khi nền di chuyển ra khỏi màn hình, đặt lại vị trí của nó
            if (bg.transform.position.x < -backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x)
            {
                float rightmostPosition = GetRightmostBackgroundPosition();
                bg.transform.position = new Vector3(rightmostPosition + backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x, 0, 0);
            }
        }

        // Giảm thời gian chờ giữa các lần tăng tốc độ
        timer -= Time.deltaTime;

        // Khi thời gian chờ hết, tăng tốc độ và reset timer
        if (timer <= 0)
        {
            moveSpeed += speedIncreaseAmount;
            timer = increaseInterval;
        }
    }

    // Tìm vị trí của nền ở xa bên phải nhất
    float GetRightmostBackgroundPosition()
    {
        float rightmostPosition = float.NegativeInfinity;
        foreach (GameObject bg in backgrounds)
        {
            if (bg.transform.position.x > rightmostPosition)
            {
                rightmostPosition = bg.transform.position.x;
            }
        }
        return rightmostPosition;
    }
}