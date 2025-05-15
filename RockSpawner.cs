using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    // エリアの範囲を指定
    public Vector2 areaSize = new Vector2(5f, 5f); // 幅と高さ
    public Transform areaCenter; // エリアの中心
    public GameObject rockPrefab; // 岩のプレハブ
    public float spawnInterval = 1f; // 岩を生成する間隔（秒）
    private float spawnTimer; //岩の発生間隔


    
    void Update()
    {
        

        // 一定時間ごとに岩を生成
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnRock();
            spawnTimer = 0f;
        }

        
    }

    void SpawnRock()
    {
        if (rockPrefab == null || areaCenter == null) return;

        // ランダムな位置を計算
        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float y = Random.Range(-areaSize.y / 2, areaSize.y / 2);

        Vector3 spawnPosition = new Vector3(areaCenter.position.x + x, areaCenter.position.y + y, 0);

        // 岩を生成
        Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
    }

    // Gizmosを使ってエリアを視覚化
    private void OnDrawGizmos()
    {
        if (areaCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(areaCenter.position, new Vector3(areaSize.x, areaSize.y, 0));
        }
    }

    
}
