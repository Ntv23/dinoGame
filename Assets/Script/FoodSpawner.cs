using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject[] foodPrefabs; // Mảng chứa các prefab của food
    public Ground ground;            // Tham chiếu đến đối tượng nền để lấy tốc độ di chuyển
    public float spawnInterval = 5f; // Khoảng thời gian giữa các lần spawn

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnFood();
            timer = spawnInterval;
        }
    }

    void SpawnFood()
    {
        // Chọn ngẫu nhiên một prefab từ danh sách
        int randomIndex = Random.Range(0, foodPrefabs.Length);
        GameObject selectedPrefab = foodPrefabs[randomIndex];

        // Tính toán vị trí spawn
        Vector3 spawnPosition = new Vector3(transform.position.x, Random.Range(-2.5f, 2.5f), 0);

        // Instantiate food và lấy component FoodController để thiết lập tốc độ di chuyển
        GameObject newFood = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        FoodController foodController = newFood.GetComponent<FoodController>();

        if (foodController != null)
        {
            // Gán tốc độ di chuyển của food bằng tốc độ của ground
            foodController.moveSpeed = ground.moveSpeed;
        }
        else
        {
            Debug.LogWarning("Food prefab is missing FoodController component!");
        }
    }
}