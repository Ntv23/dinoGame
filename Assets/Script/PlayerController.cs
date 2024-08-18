using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Facebook.Unity;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rbPlayer;
    public float jumpForce = 15f;
    public bool isJumping;

    public GameObject GameOverCanvas;
    public bool isGameOver = false;
    public TextMeshProUGUI gameOverTimeText; // Text để hiển thị thời gian khi game over

    public TextMeshProUGUI timerText;
    public float gameTime = 0f;
    public float timeBonus = 2f;
    public float weightIncrease = 0.2f;

    public Animator anim;

    private DatabaseReference databaseReference;

    void Start()
    {
        // Khởi tạo các biến và ẩn Canvas Game Over khi bắt đầu
        GameOverCanvas.SetActive(false);
        rbPlayer = GetComponent<Rigidbody2D>();

        // Khởi tạo Firebase Database
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                // Cấu hình URL của Firebase Realtime Database ở đây
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError($"Failed to initialize Firebase with error: {task.Exception}");
            }
        });
    }

    void Update()
    {
        // Cập nhật thời gian chơi nếu chưa kết thúc
        if (!isGameOver)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }

        anim.SetBool("isJumping", isJumping);

        // Xử lý nhảy khi nhấn phím Jump và không phải khi đã kết thúc
        if (Input.GetButtonDown("Jump") && !isJumping && !isGameOver)
        {
            Jump();
        }
    }

    public void Jump()
    {
        // Xử lý nhảy của player
        if (!isJumping && !isGameOver)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpForce);
            isJumping = true;
            anim.SetBool("isJumping", true);
            Debug.Log("Jump");
        }
    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        // Xử lý khi player va chạm với ground
        if (target.gameObject.CompareTag("ground"))
        {
            isJumping = false;
            Debug.Log("no jump");
        }

        // Xử lý khi player va chạm với obstacles
        if (target.gameObject.CompareTag("obtacles"))
        {
            GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D target1)
    {
        // Xử lý khi player va chạm với food
        if (target1.gameObject.CompareTag("food"))
        {
            IncreaseTimeAndWeight();
            Destroy(target1.gameObject);
        }
    }

    void GameOver()
    {
        // Xử lý khi game over
        isGameOver = true;
        Time.timeScale = 0;
        GameOverCanvas.SetActive(true);
        Debug.Log("GAME OVER!");
        SaveGameTimeToFirebase(); // Lưu thời gian chơi khi kết thúc game
        DisplayGameOverTime(); // Hiển thị thời gian khi game over
    }

    void UpdateTimerUI()
    {
        // Cập nhật giao diện hiển thị thời gian chơi
        float minutes = Mathf.Floor(gameTime / 60);
        float seconds = Mathf.Floor(gameTime % 60);
        float milliseconds = Mathf.Floor((gameTime * 1000) % 1000 / 10);
        timerText.text = string.Format("{0:0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    void IncreaseTimeAndWeight()
    {
        // Tăng thời gian chơi và trọng lượng của player khi ăn food
        gameTime += timeBonus;
        rbPlayer.gravityScale += weightIncrease;
    }

    void SaveGameTimeToFirebase()
    {
        FirebaseDatabase database = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, "https://vrex-project-default-rtdb.firebaseio.com/");
        if (database != null)
        {
            string userId;
            if (FB.IsLoggedIn)
            {
                userId = AccessToken.CurrentAccessToken.UserId;
            }
            else if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            {
                userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            }
            else
            {
                Debug.LogError("User is not authenticated.");
                return;
            }

            DatabaseReference reference = database.RootReference.Child("users").Child(userId);
            reference.Child("maxGameTime").GetValueAsync().ContinueWith(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    float maxTime = float.MinValue;

                    if (snapshot.Exists)
                    {
                        maxTime = float.Parse(snapshot.Value.ToString());
                    }

                    if (gameTime > maxTime)
                    {
                        reference.Child("maxGameTime").SetValueAsync(gameTime).ContinueWith(saveTask => {
                            if (saveTask.IsCompleted)
                            {
                                Debug.Log("Max game time updated to Firebase.");
                            }
                            else if (saveTask.IsFaulted)
                            {
                                Debug.LogError("Failed to update max game time to Firebase: " + saveTask.Exception);
                            }
                        });
                    }

                    reference.Child("username").GetValueAsync().ContinueWith(usernameTask => {
                        if (usernameTask.IsCompleted && !usernameTask.Result.Exists)
                        {
                            string username = PlayerPrefs.GetString("username");
                            reference.Child("username").SetValueAsync(username).ContinueWith(setUsernameTask => {
                                if (setUsernameTask.IsCompleted)
                                {
                                    Debug.Log("Username set to Firebase.");
                                }
                                else if (setUsernameTask.IsFaulted)
                                {
                                    Debug.LogError("Failed to set username to Firebase: " + setUsernameTask.Exception);
                                }
                            });
                        }
                    });
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve max game time from Firebase: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("Failed to get FirebaseDatabase instance. Check your Firebase Database URL configuration.");
        }
    }

    void DisplayGameOverTime()
    {
        // Hiển thị thời gian chơi khi game over
        gameOverTimeText.text = timerText.text;
    }
}
