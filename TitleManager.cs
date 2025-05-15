using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class TitleManager : MonoBehaviour
{
    public InputField nameInputField; //名前入力欄参照
    public Button startButton; //スタートボタン参照
    public SceanManager sceanManager; //SceanManagerスクリプト参照

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);//ボタンが押されたらスタートボタン関数を呼ぶ

        Application.targetFrameRate = 60; //60fps

    }
 //スタートボタン関数
    void OnStartButtonClicked()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName)) //もし名前が入力されていなかったら
        {
            Debug.LogWarning("名前が入力されていません！");
            return;
        }

        // ログインしてから名前登録
        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = nameInputField.text,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(loginRequest, result =>
        {
            Debug.Log("ログイン成功");

            // ログイン成功後に表示名を設定
            var displayNameRequest = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = playerName 
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, //PlayFabに名前登録
                updateResult => 
                {
                    Debug.Log("名前登録成功！");
                    sceanManager.ChangeGameModeScene();
                   
                },
                error =>
                {
                    Debug.LogError("名前登録失敗: " + error.GenerateErrorReport());
                });

        }, error =>
        {
            Debug.LogError("ログイン失敗: " + error.GenerateErrorReport());
        });
    }
}

