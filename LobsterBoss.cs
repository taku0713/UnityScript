using System.Collections;
using UnityEngine;

public class LobsterBoss : MonoBehaviour
{
    public float speed = 5f;             // 突進速度
    public float stopTime = 2f;          // 壁にぶつかった後の停止時間
    public float dashRange = 5f;         // プレイヤーがこの距離内にいると突進開始

    private Transform player; //プレイヤーの位置参照
    private Rigidbody2D rb; //リジットボディ取得用
    private bool isDashing = false; //追いかける状態設定
    private Vector2 dashDirection;

    public int HP = 3; //ボスのHP
    public float flashDuration = 0.1f; //光る間隔設定
    public int flashCount = 3; //何回光るか設定

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public ParticleSystem DieEffect; //ボス撃墜エフェクト参照

   public GameManager gameManager;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //プレイヤーの位置取得
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DashLogic());

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
//プレイヤーを追いかける関数
    IEnumerator DashLogic()
    {
        while (true)
        {
            yield return new WaitForSeconds(stopTime); //壁にぶつかったら少し止まる

            if (player != null && !isDashing)
            {
                float distance = Vector2.Distance(transform.position, player.position); //プレイヤーまでの距離

                if (distance <= dashRange)
                {
                    // プレイヤーの方向を計算
                    dashDirection = (player.position - transform.position).normalized;

                    // スプライトが上向きなので -90度補正
                    float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg - 90f;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    // 突進開始
                    rb.linearVelocity = dashDirection * speed;
                    isDashing = true;
                }
            }
        }
    }
//壁にぶつかる判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Trap"))
        {
            rb.linearVelocity = Vector2.zero;
            isDashing = false;
            // 停止後は DashLogic の中で再チェックする
        }
    }

//ボスのHP管理
     public void TakeDamage(int damage)
    {
        HP -= damage;
        Debug.Log(gameObject.name + "はダメージを受けた");
        StartCoroutine(FlashEffect());

        if(HP <= 0)
        {
            Die();
        }
    }
    //ボス撃墜関数
    private void Die()
    {
     ParticleSystem newParticle = Instantiate(DieEffect);
     // パーティクルの発生場所をこのスクリプトをアタッチしているGameObjectの場所にする。
     newParticle.transform.position = this.transform.position;
     // パーティクルを発生させる。
      newParticle.Play();
      Debug.Log(gameObject.name + "を倒した");
      Destroy(gameObject);
      gameManager.GameClear(); //ゲームクリア関数を呼ぶ
    }
    void OnDrawGizmosSelected() //プレイヤーを追いかける範囲を可視化
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, dashRange);
}
private IEnumerator FlashEffect()  //ボスがダメージを受けたら発光
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); // 半透明
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

}
