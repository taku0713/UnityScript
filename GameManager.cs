using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class GameManager : MonoBehaviour
{
         
    
    public GameObject gameOverMenu;  //ゲームオーバー画面参照
    public GameObject gameClear;  //ゲームクリア画面参照
    public GameObject deathParticlePrefab;  // 死亡パーティクルのプレハブ参照
    private bool isGameOver = false;  // ゲームオーバーフラグ設定

    private bool isGameCrear = false; //ゲームクリアフラグ設定
    public GameObject player;  // プレイヤー参照
    public Animator GameOveranimator; // Animator参照
    public Animator GameClearanimator; //ゲームクリアアニメーション参照
    public AudioSource RestartAudio1; //ボタンを押した時の効果音参照
    public AudioSource RestartAudio2; //ボタンを押した時の効果音参照

    public AudioClip GameOverAudio; //ゲームオーバーの効果音参照
    public AudioClip GameClearAudio; //ゲームクリアの効果音参照
    private AudioSource audioSource;    // AudioSourceコンポーネント取得

    public GameObject attackButtonObject; //　攻撃ボタン参照

    public Text resultText;           // 最終タイム表示テキスト参照

    public TimeTracker timeTracker; //TimeTrackerスクリプト参照
    public PlayFabManager playFabManager; // PlayFab用スクリプト参照

    void Start()
    {
       Application.targetFrameRate = 60; //フレームレート60fpsに設定

       Time.timeScale = 1f; //ゲーム再生
        

        
        gameOverMenu.SetActive(false);//最初はゲームオーバーパネルを非表示
        gameClear.SetActive(false);//最初はゲームクリアパネルを非表示
        
       //AudioSourceコンポーネント取得
       audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //ゲームクリア画面のリスタートボタンのSE再生
    public void GameClearRestartSE()
    {
     if(RestartAudio1 != null)
     {
      RestartAudio1.Play();
     }
    }
    //ゲームオーバー画面のリスタートボタンのSE再生
    public void GameOverRestartSE()
    {
     if(RestartAudio2 != null)
     {
      RestartAudio2.Play();
     }
    }
    
    //ゲームクリア関数
    public void GameClear()
    {
      if(isGameCrear || isGameOver) return;
      isGameCrear = true;
      attackButtonObject.SetActive(false); // ボタンそのものを非表示にする

      

      StartCoroutine(ShowGameClearText());

      //ゲームクリアSE再生
      if(GameClearAudio != null && audioSource != null)
      {
         audioSource.PlayOneShot(GameClearAudio);

      }
        //クリアタイム計測
        if (timeTracker != null)
        {
            timeTracker.StopTimer();
            float clearTime = timeTracker.GetElapsedTime();
            resultText.text = "クリアタイム: " + clearTime.ToString("F2") + " 秒";


        
        int clearTimeInt = Mathf.FloorToInt(clearTime * 100); // 秒を小数2桁」でintに変換する

        Debug.Log("クリアタイム（送信用）: " + clearTimeInt);

        playFabManager.TrySubmitScoreIfTop5(clearTimeInt); // PlayFabに送信
        }


    }

    //ゲームオーバー関数
    public void GameOver()
    {
     if (isGameOver || isGameCrear) return;  // すでにゲームオーバーなら処理しない
        isGameOver = true;

       attackButtonObject.SetActive(false); // ボタンそのものを非表示にする


        // プレイヤーの位置に死亡パーティクルを生成
        Instantiate(deathParticlePrefab, player.transform.position, Quaternion.identity);

        // プレイヤーを非表示にする
        player.SetActive(false);

        

       // 3秒後にゲームオーバーのテキストを表示
        StartCoroutine(ShowGameOverText());

      // ゲームオーバオーディオを再生
        if (GameOverAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(GameOverAudio);
        }


       Time.timeScale = 0f; //ゲーム停止

       
    }
  //ゲームオーバーテキスト　表示
    private IEnumerator ShowGameOverText()
   {
        yield return new WaitForSecondsRealtime(2.0f);  // 3秒待つ

       gameOverMenu.SetActive(true);
       GameOveranimator.SetTrigger("OMG");
      
   }
  //ゲームクリアテキスト　表示
   private IEnumerator ShowGameClearText()
   {
        yield return new WaitForSecondsRealtime(2.0f);  // 3秒待つ

       gameClear.SetActive(true); 
       GameClearanimator.SetTrigger("GG");

      

      Time.timeScale = 0; //ゲーム停止
      
   }
   
}
