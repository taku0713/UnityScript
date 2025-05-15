using UnityEngine;

public class TitleBubbleManager : MonoBehaviour
{
    public GameObject bubblePrefab; // 泡のプレハブ
    public float spawnInterval = 1f; // 泡を生成する間隔


    void Start()
    {
        Time.timeScale = 1f; //止まったゲームを再生
        InvokeRepeating(nameof(SpawnBubble), 0f, spawnInterval); //一定時間ごとに泡生成関数を呼ぶ
    }
//泡生成関数
    void SpawnBubble()
    {
        // 泡をランダムな位置に生成
        Vector3 spawnPosition = new Vector3(
            Random.Range(-10f, 10f), // X座標の範囲
            transform.position.y,
            0
        );
        Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    }
}
