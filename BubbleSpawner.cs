using UnityEngine;
using Cinemachine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab; // 泡のプレハブ参照
    public float spawnInterval = 1f; // 泡を生成する間隔
    public CinemachineVirtualCamera virtualCamera; // Virtual Cameraの参照

    private Camera mainCamera; //メインカメラ参照

    void Start()
    {
        mainCamera = Camera.main; //メインカメラ取得
        InvokeRepeating(nameof(SpawnBubble), 0f, spawnInterval); //一定秒ごとに泡を生成
    }

    void SpawnBubble()
    {
        // Cinemachineカメラの範囲を取得
        float orthographicSize = virtualCamera.m_Lens.OrthographicSize;
        float aspectRatio = mainCamera.aspect;
        float cameraHeight = 2f * orthographicSize;
        float cameraWidth = cameraHeight * aspectRatio;

        // ランダムな位置を生成（カメラ内）
        float randomX = Random.Range(-cameraWidth / 2f, cameraWidth / 2f);
        float randomY = -cameraHeight / 2f; // カメラの下端で発生
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        // カメラの位置にオフセットを加える
        spawnPosition += virtualCamera.transform.position;

        // 泡を生成
        Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    }
}
