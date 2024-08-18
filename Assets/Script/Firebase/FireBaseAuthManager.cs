using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;
using TMPro;
using Firebase.Database;
using System;

public class FireBaseAuthManager : MonoBehaviour
{
    [Header("Login Fields")]
    public TMP_InputField loginUsernameField; // Sử dụng để nhập username hoặc email
    public TMP_InputField loginPasswordField;
    public TextMeshProUGUI loginMessageText;

    [Header("Register Fields")]
    public GameObject registerPanel;
    public TMP_InputField registerUsernameField; // Sử dụng để nhập username
    public TMP_InputField registerEmailField;    // Sử dụng để nhập email
    public TMP_InputField registerPasswordField;
    public TMP_InputField registerConfirmPasswordField;
    public TextMeshProUGUI registerMessageText;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        registerPanel.SetActive(false); // Ẩn panel đăng ký ban đầu
    }

    public async void RegisterUser()
    {
        string email = registerEmailField.text;
        string username = registerUsernameField.text;
        string password = registerPasswordField.text;
        string confirmPassword = registerConfirmPasswordField.text;

        if (password != confirmPassword)
        {
            registerMessageText.text = "Passwords do not match!";
            return;
        }

        try
        {
            // Đăng ký người dùng với email và password
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = authResult.User;

            if (newUser != null)
            {
                // Cập nhật profile để lưu trữ username
                UserProfile profile = new UserProfile { DisplayName = username };
                await newUser.UpdateUserProfileAsync(profile);

                // Lưu username vào database
                DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(newUser.UserId);
                await reference.Child("username").SetValueAsync(username);

                registerMessageText.text = "Registration successful!";
                registerPanel.SetActive(false); // Ẩn panel đăng ký sau khi đăng ký thành công
            }
            else
            {
                registerMessageText.text = "Registration failed!";
            }
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Registration failed: {ex.Message}");
            registerMessageText.text = "Registration failed!";
        }
    }

    public async void LoginUser()
    {
        string email = loginUsernameField.text;
        string password = loginPasswordField.text;

        try
        {
            // Đăng nhập người dùng với email và password
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            if (user != null)
            {
                // Lấy username từ email
                string username = email.Substring(0, email.IndexOf('@'));

                // Lưu username vào PlayerPrefs và database
                PlayerPrefs.SetString("username", username);

                DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(user.UserId);
                await reference.Child("username").SetValueAsync(username);

                loginMessageText.text = "Login successful!";
                LoadGameplayScene(username);
            }
            else
            {
                loginMessageText.text = "Login failed!";
            }
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            loginMessageText.text = "Login failed!";
        }
    }

    void LoadGameplayScene(string username)
    {
        PlayerPrefs.SetString("username", username);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

    public void HideRegisterPanel()
    {
        registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        registerPanel.SetActive(true);
    }

    public void LoginWithFacebook(string tokenString, string username, string email)
    {
        Credential credential = FacebookAuthProvider.GetCredential(tokenString);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;

            if (newUser != null)
            {
                // Check if the user is new or existing
                bool isNewUser = task.Result.Metadata.CreationTimestamp == task.Result.Metadata.LastSignInTimestamp;

                if (isNewUser)
                {
                    // This is a new user, save their details to Firebase
                    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(newUser.UserId);
                    reference.Child("username").SetValueAsync(username);
                    reference.Child("email").SetValueAsync(email);
                }

                // Proceed to load the gameplay scene
                LoadGameplayScene(username);
            }
            else
            {
                Debug.LogError("Facebook login failed for some reason...");
            }
        });
    }

}
