using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject cloudPrefab; // Prefab của đám mây
    public int initialCloudCount = 5; // Số lượng đám mây ban đầu
    public float cloudSpeed = 2f; // Tốc độ di chuyển của đám mây
    public float spawnXPosition = 10f; // Vị trí x để spawn đám mây mới
    public float despawnXPosition = -10f; // Vị trí x để tái sử dụng đám mây
    public float minYPosition = 2.5f; // Vị trí y tối thiểu
    public float maxYPosition = 4.25f; // Vị trí y tối đa

    private List<GameObject> clouds = new List<GameObject>();

    private void Start()
    {
        // Khởi tạo các đám mây ban đầu
        for (int i = 0; i < initialCloudCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnXPosition, spawnXPosition), Random.Range(minYPosition, maxYPosition), 0);
            GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);
            clouds.Add(cloud);
        }
    }

    private void Update()
    {
        // Di chuyển các đám mây
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.Translate(Vector3.left * cloudSpeed * Time.deltaTime);

            // Kiểm tra nếu đám mây ra khỏi màn hình
            if (cloud.transform.position.x < despawnXPosition)
            {
                // Tái sử dụng đám mây
                float newYPosition = Random.Range(minYPosition, maxYPosition);
                cloud.transform.position = new Vector3(spawnXPosition, newYPosition, 0);
            }
        }
    }
}
