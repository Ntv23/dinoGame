using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;

    private void Start()
    {
        // Thiết lập màn hình ngang
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        string username = PlayerPrefs.GetString("username", "Guest");
        usernameText.text = "Hi, " + username;
    }

}
