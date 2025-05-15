using UnityEngine;

public class LeafFalling : MonoBehaviour
{
    public float fallSpeed = 1f; // 落下速度
    public float swayAmount = 1f; // 横揺れの大きさ
    public float swaySpeed = 2f; // 横揺れの速さ

    private float startX; //初期位置設定
    private float time;  //時間計測用

    void Start()
    {
        startX = transform.position.x;
        time = Random.Range(0f, 2f); // ずれを作る
    }

    void Update()
    {
        // 時間経過で左右にスイングさせる
        float sway = Mathf.Sin(time * swaySpeed) * swayAmount;
        transform.position += new Vector3(sway * Time.deltaTime, -fallSpeed * Time.deltaTime, 0);
        
        time += Time.deltaTime;
    }
}
