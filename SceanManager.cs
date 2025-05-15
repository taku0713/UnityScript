using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceanManager : MonoBehaviour
{
    public string GameScene; //ゲームシーン参照
    public string TitleScene; //タイトルシーン参照
    public float delay = 0.5f; //シーン変異ロード時間

    public AudioSource StartAudio; //スタートボタンSE

    //ゲームシーン再生
    public void ChangeGameModeScene()
    {

        StartCoroutine(ChangeGame());
    }
　　　//タイトルシーンに移行
     public void ChangeTitleScene()
    {
        StartCoroutine(ChangeTitle());
    }
//ロード時間
    private IEnumerator ChangeGame()
    {       

       yield return new WaitForSecondsRealtime(delay); // 遅延時間を待つ


        SceneManager.LoadScene(GameScene);
    }
//ロード時間
    private IEnumerator ChangeTitle()
    {
    yield return new WaitForSecondsRealtime(delay); // 遅延時間を待つ

     SceneManager.LoadScene(TitleScene);
    }

    public void RestartSE()
    {
     if(StartAudio != null)
     {
      StartAudio.Play();
     }
    }
}
