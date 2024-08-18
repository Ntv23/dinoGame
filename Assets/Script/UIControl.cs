using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tryagain()
    {
        Time.timeScale = 1; // Đặt lại Time.timeScale để trò chơi tiếp tục chạy bình thường
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại cảnh hiện tại
    }
    public void leaderBoard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
