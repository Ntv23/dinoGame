using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtaclesScript : MonoBehaviour
{
    public Ground ground; // Tham chiếu đến đối tượng nền

    void Update()
    {
        if (ground != null)
        {
            // Di chuyển chướng ngại vật theo trục x với tốc độ của nền
            transform.Translate(Vector3.left * ground.moveSpeed * Time.deltaTime);

            // Khi chướng ngại vật di chuyển ra khỏi camera, reset vị trí
            if (transform.position.x < -20f)
            {
                Destroy(gameObject);
            }
        }
    }
}