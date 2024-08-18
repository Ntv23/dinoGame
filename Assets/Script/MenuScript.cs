using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject loginCanvas;
    public GameObject signupCanvas;
    public Button facebookLoginButton;
    public Button guestButton;

    public FireBaseAuthManager authManager;

    private void Start()
    {
        // Thiết lập màn hình dọc
        Screen.orientation = ScreenOrientation.Portrait;

        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);

        facebookLoginButton.onClick.AddListener(FacebookLogin);
        guestButton.onClick.AddListener(GuestLogin);

        // Initialize Facebook SDK
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    Debug.LogError("Failed to initialize the Facebook SDK");
                }
            });
        }
    }

    public void FacebookLogin()
    {
        // Xử lý đăng nhập Facebook
        List<string> permissions = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, OnFacebookSignIn);
    }

    private void OnFacebookSignIn(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            string accessToken = token.TokenString;

            FB.API("/me?fields=name,email", HttpMethod.GET, OnFacebookUserInfo);
        }
        else
        {
            Debug.LogError("Facebook Sign-In failed.");
        }
    }

    void OnFacebookUserInfo(IGraphResult result)
    {
        if (result.Error == null)
        {
            IDictionary<string, object> profile = result.ResultDictionary;

            if (profile.ContainsKey("name"))
            {
                PlayerPrefs.SetString("username", profile["name"].ToString());
            }

            // Check if email exists in the result dictionary
            if (profile.ContainsKey("email"))
            {
                string email = profile["email"].ToString();

                // Save Facebook login information to Firebase
                FireBaseAuthManager authManager = FindObjectOfType<FireBaseAuthManager>();
                authManager.LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString, profile["name"].ToString(), email);
            }
            else
            {
                Debug.LogWarning("Email not found in Facebook user info.");
            }

            SceneManager.LoadScene("GamePlay");
        }
        else
        {
            Debug.LogError("Failed to get Facebook user info: " + result.Error);
        }
    }


    public void GuestLogin()
    {
        // Xử lý chơi khách
        PlayerPrefs.SetString("username", "Guest");
        SceneManager.LoadScene("GamePlay");
    }

    public void openPanel()
    {
        loginCanvas.SetActive(true);
    }

    public void signupPanel()
    {
        signupCanvas.SetActive(true);
    }

    public void closePanel()
    {
        loginCanvas.SetActive(false);
    }

    public void closePanelSignup()
    {
        signupCanvas.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void OnRegisterButtonClicked()
    {
        authManager.RegisterUser();
    }

    public void OnLoginButtonClicked()
    {
        authManager.LoginUser();
    }
}
