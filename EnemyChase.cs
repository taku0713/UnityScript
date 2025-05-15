using UnityEngine;
using System.Collections;

public class EnemyChase : MonoBehaviour
{
    public Transform player;         //プレイヤーのトランスフォーム参照
    public float chaseSpeed = 2.0f;  //追いかける速度
    public float chaseRange = 5.0f;  //プレイヤーを見つける範囲1
    public float stopRange = 1.0f;   //追いかける範囲
    public float maxChaseRange = 7.0f; //プレイヤーを見つける範囲2

    private SpriteRenderer spriteRenderer;  //スプライト参照
    private bool isChasing = false; //追いかける状態設定

    public int HP = 3;//敵のHP
    public float flashDuration = 0.1f; //光る間隔設定
    public int flashCount = 3; //何回光るか設定
    private Color originalColor;



    void Start()
    {
        //プレイヤーがどこにいるか取得
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        

        //スプライトコンポーネント取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;  //スプライトカラー設定

    }

    void Update()
    {
        //プレイヤーの位置
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        //プレイヤーが範囲内だったら
        if (distanceToPlayer < chaseRange && distanceToPlayer > stopRange)
        {
            isChasing = true;
        }
　　　　　//プレイヤーが範囲外だったら
        else if (distanceToPlayer <= stopRange)
        {
            isChasing = false;
        }
        //プレイヤーが範囲外だったら
        else if (distanceToPlayer > maxChaseRange)
        {
            isChasing = false;
        }

        //isChasingがtrueだったらChasePlayer関数を呼ぶ
        if (isChasing)
        {
            ChasePlayer();
        }
    }


    //プレイヤーを追いかける関数
    void ChasePlayer()
    {
        
        Vector2 direction = (player.position - transform.position).normalized;

        
        if (direction.x < 0) 
        {
            spriteRenderer.flipX = false;  
        }
        else if (direction.x > 0) 
        {
            spriteRenderer.flipX = true;   
        }

        
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    

     //追いかける範囲を可視化　
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);  
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopRange);   
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxChaseRange); 
    }

    //敵のHP管理関数
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

    //敵撃墜関数
    private void Die()
    {
      Debug.Log(gameObject.name + "を倒した");
      Destroy(gameObject);
    }
    //ダメージを受けた時に光る関数
 private IEnumerator FlashEffect()
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




