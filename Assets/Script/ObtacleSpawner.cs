using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // Mảng chứa các prefab của chướng ngại vật
    public Ground ground; // Tham chiếu đến đối tượng nền
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
            SpawnObstacle();
            timer = spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        // Chọn ngẫu nhiên một prefab từ danh sách
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0);
        GameObject newObstacle = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        newObstacle.GetComponent<ObtaclesScript>().ground = ground; // Gán tham chiếu đối tượng nền cho chướng ngại vật mới
    }
}