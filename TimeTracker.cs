using UnityEngine;
using UnityEngine.UI;

public class TimeTracker : MonoBehaviour
{
    public Text timeText; // 経過時間表示用のText
    private float elapsedTime = 0f; //初期タイムカウント０
    private bool isTiming = true; //

    void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime; //時間計測
            timeText.text = "Time: " + elapsedTime.ToString("F2") + "s";
        }
    }
//時間計測停止
    public void StopTimer()
    {
        isTiming = false;
    }
//計測タイムを取得
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}

