using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class LeaderBoardManager : MonoBehaviour
{
    DatabaseReference databaseReference;
    public TextMeshProUGUI leaderboardText;

    void Start()
    {
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                // Sử dụng URL của Firebase Realtime Database từ Firebase Console
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                RetrieveLeaderboardData();
            }
            else
            {
                Debug.LogError($"Failed to initialize Firebase with error: {task.Exception}");
            }
        });
    }

    void RetrieveLeaderboardData()
    {
        // Lấy dữ liệu từ Firebase Realtime Database
        databaseReference.Child("users").OrderByChild("maxGameTime").ValueChanged += HandleLeaderboardValueChanged;
    }

    void HandleLeaderboardValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError($"Database error: {args.DatabaseError.Message}");
            return;
        }

        if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
        {
            List<UserData> leaderboard = new List<UserData>();
            foreach (var childSnapshot in args.Snapshot.Children)
            {
                UserData user = new UserData();
                user.userId = childSnapshot.Key;
                user.maxGameTime = Convert.ToSingle(childSnapshot.Child("maxGameTime").Value);
                user.username = childSnapshot.Child("username").Value.ToString();
                leaderboard.Add(user);
            }

            // Sắp xếp danh sách người chơi theo thời gian chơi giảm dần
            leaderboard.Sort((x, y) => y.maxGameTime.CompareTo(x.maxGameTime));

            // Hiển thị danh sách lên UI Text
            leaderboardText.text = "";
            for (int i = 0; i < leaderboard.Count && i < 100; i++)
            {
                leaderboardText.text += $"{i + 1}. {leaderboard[i].username} - {leaderboard[i].maxGameTime}\n";
            }
        }
        else
        {
            Debug.LogWarning("No leaderboard data available.");
        }
    }

    // Class UserData để lưu trữ dữ liệu từ Firebase
    public class UserData
    {
        public string userId;
        public float maxGameTime;
        public string username;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
