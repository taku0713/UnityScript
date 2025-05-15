using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public float speed;  //移動速度
    private float currentSpeed;
    public float boostDuration = 1f; // 加速する時間（秒）
    public float boostedSpeed = 10f; //加速中の速度
    public VariableJoystick variableJoystick;  //ジョイスティック
    private bool isFacingRight = false;  // 初めは左を向いている
    public GameManager gameManager; //GameManagerスクリプト参照
    public float timeBonus = 10.0f; // 制限時間追加
    private Animator animator;
    private bool isDead = false;  // 死亡フラグ
    private AudioSource audioSource;
    public AudioClip EatAudio; //食べる効果音
    //攻撃用変数
    public float headbuttForce = 10f; // 頭突きの勢い
    public float headbuttDuration = 0.2f; // 頭突きの持続時間
    public LayerMask enemyLayer; // 攻撃対象
    public Transform attackPoint; // 頭突きの当たり判定位置
   public float recoilForce = 5f; // 後ろに下がる力
    public float recoilDuration = 0.1f; // 後退の持続時間
    private Rigidbody2D rb;
    private bool isHeadbutting = false;
    private bool canHeadbutt = false; // 頭突き可能フラグ
    private Vector2 originalVelocity;
    public GameObject headbuttEffectPrefab;
    public GameObject levelUpEffectPrefab;  // レベルアップのエフェクト
public AudioClip levelUpSound;          // レベルアップ音
public AudioClip AttackSound;        //攻撃の音
public Transform effectSpawnPoint;      // エフェクトを出す場所
public GameObject attackButtonObject; // ButtonのGameObject全体（例：AttackButton）
public EatEffect eatEffect;

private int currentLevelIndex = -1;     // 現在のレベル段階（最初は未設定）
    
    [System.Serializable]
    public class AttackRangeLevel //パワーアップレベル
    {
      public int requiredItems;   // この段階に必要なアイテム数
      public float radius;        // この段階の攻撃範囲
    }

    public List<AttackRangeLevel> attackRangeLevels = new List<AttackRangeLevel>();

    public int itemCount = 0; // 現在のアイテム数

    

    void Start()
    {
        currentSpeed = speed; //初期スピードを保存
        
        
        animator = GetComponent<Animator>(); //アニメーターコンポーネント取得

        audioSource = gameObject.GetComponent<AudioSource>(); //AudioSourceコンポーネント取得
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rb = GetComponent<Rigidbody2D>(); //プレイヤーのリジットボディボディ取得

    }
    void Update()
    {

    if (Input.GetKeyDown(KeyCode.Space))
    {
        OnAttackButtonPressed(); // ボタンと同じ処理を呼ぶ
    }


        if (isDead) return;  // 死亡後は操作しない

       // JoystickとKeyboardの両方の入力を取得
        float horizontal = variableJoystick.Horizontal + Input.GetAxisRaw("Horizontal");
        float vertical = variableJoystick.Vertical + Input.GetAxisRaw("Vertical");

        // 移動の数値を取得
        Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;

        // プレイヤーを移動させる
        this.transform.position += direction * speed * Time.deltaTime;

        // 向きを判定
        if (direction.x < 0 && !isFacingRight)
        {
            Flip();  // 右を向く
        }
        else if (direction.x > 0 && isFacingRight)
        {
            Flip();  // 左を向く
        }

        // 移動速度を計算してアニメーターに設定
        float moveSpeed = direction.magnitude;  // ベクトルの長さで速度を計算
        animator.SetFloat("Speed", moveSpeed);

　　　    　// サメの位置に合わせてアタックポイントを更新
        attackPoint.position = transform.position + new Vector3(transform.localScale.x * -1.3f, 0, 0);

       

    }

    public void OnAttackButtonPressed() //攻撃ボタンをタップ(画面右半分)
{
    if (!gameObject.activeInHierarchy) return; 
    if (canHeadbutt && !isHeadbutting)
    {
        StartCoroutine(Headbutt());
    }
}
    //頭突き関数
     private IEnumerator Headbutt()
    {
        isHeadbutting = true; //頭突き可能
        originalVelocity = rb.linearVelocity;

        // 向いている方向に基づいて突進方向を決定
        float direction = transform.localScale.x > 0 ? -1f : 1f; 
        Vector2 headbuttDirection = new Vector2(direction, 0); // 水平方向のみ
        rb.linearVelocity = headbuttDirection * headbuttForce;

        yield return new WaitForSeconds(headbuttDuration);
        
       // エフェクトを生成＆再生
    if (headbuttEffectPrefab != null)
    {
        GameObject effectInstance = Instantiate(headbuttEffectPrefab, attackPoint.position, Quaternion.identity); //攻撃生成
        // ここでスケールを調整（例：Lv1=1.0, Lv2=1.5, Lv3=2.0）
        float scaleMultiplier = 1.0f + (currentLevelIndex - 1) * 0.5f;
        effectInstance.transform.localScale *= scaleMultiplier;

        ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>(); //攻撃パーティクル取得

        if (ps != null)
        {
            ps.Play();
            Destroy(effectInstance, ps.main.duration); // エフェクトが終わったら削除
        }
        
        if(AttackSound != null && audioSource != null) //攻撃効果音再生
        {
            audioSource.PlayOneShot(AttackSound);
        }
    // 🦈 頭突きで敵を判定
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, GetCurrentAttackRadius());
    foreach (Collider2D enemy in hitEnemies)
    {
        if (enemy.CompareTag("Enemy")) //サメを攻撃したら
        {
            EnemyChase enemyScript = enemy.GetComponent<EnemyChase>();
            LobsterBoss bossScript = enemy.GetComponent<LobsterBoss>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(1); //サメにダメージ
            }
            if (bossScript != null)
            {
                bossScript.TakeDamage(1);//ボスにダメージ
            }
        }
    }

    }
        // 頭突き後の後退処理
        rb.linearVelocity = -headbuttDirection * recoilForce;
        yield return new WaitForSeconds(recoilDuration);

         // 元の速度に戻す
        rb.linearVelocity = originalVelocity;
        isHeadbutting = false;

    }
   

   

    // 反転させる関数
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

　　　　//移動速度加速関数
    IEnumerator SpeedBoost()
    {    
        // 速度を上げる
        speed = boostedSpeed;

        // 指定時間待つ
        yield return new WaitForSeconds(boostDuration);

        // 元の速度に戻す
        speed = currentSpeed;

    }

    //オブジェクトに接触
    void OnCollisionEnter2D(Collision2D other)
    {
        // 敵に触れたら
        if (other.gameObject.CompareTag("Enemy"))
        {
            //ゲームオーバー
            if (gameManager != null)
            {
                Die();
                
            }
        }

        // 食べ物に触れたら
        else if (other.gameObject.CompareTag("Food"))
        {
            if(gameManager != null)
           {
              StartCoroutine(SpeedBoost()); //速度アップ関数を呼ぶ

             eatEffect.EatEffectSpown(); //食べカスのエフェクト発生
              // 効果音を再生
              if (EatAudio != null && audioSource != null)
              {
               audioSource.PlayOneShot(EatAudio, 1.0f); // 0.0（無音）～ 1.0（最大音量）
              }

              itemCount++; //食べ物取得数＋１
              UpdateLevel();
 
              if (itemCount >= 5 && !canHeadbutt) //食べ物５個食べたら
              {
                canHeadbutt = true; //頭突き可能
                Debug.Log("頭突きが可能になった！");
              }

              Destroy(other.gameObject); // 食べ物消滅
           }

        }

        //トラップに触れたら
        else if (other.gameObject.CompareTag("Trap"))
        {
           if (gameManager != null)
            {
              Die(); //死亡関数を呼ぶ
            }
        }
    }
    
    //死亡関数
    void Die()
    {
        if (isDead) return;
        isDead = true;   
        gameManager.GameOver();  // GameManager の GameOver() を呼び出す
    }

//攻撃範囲管理
   private float GetCurrentAttackRadius()
{
    float currentRadius = 0.5f; // デフォルト値

    foreach (var level in attackRangeLevels)
    {
        if (itemCount >= level.requiredItems)
        {
            currentRadius = level.radius;
        }
        else
        {
            break;
        }
    }

    return currentRadius;
}

 //攻撃範囲可視化
    private void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, GetCurrentAttackRadius());
}
//パワーアップレベル管理
private void UpdateLevel()
{
    for (int i = attackRangeLevels.Count - 1; i >= 0; i--)
    {
        if (itemCount >= attackRangeLevels[i].requiredItems)
        {
            if (currentLevelIndex < i)
            {
                // 新しいレベルに上がったら演出！
                PlayLevelUpEffect();
                currentLevelIndex = i;
            }
            break;
        }
    }
}

//レベルアップエフェクト関数
private void PlayLevelUpEffect()
{
    if (levelUpEffectPrefab != null && effectSpawnPoint != null)
    {
var effect = Instantiate(levelUpEffectPrefab, effectSpawnPoint.position, levelUpEffectPrefab.transform.rotation);//パーティクル生成

// ParticleSystem を取得して再生
ParticleSystem ps = effect.GetComponent<ParticleSystem>();
if (ps != null)
{
    ps.Play();
    Debug.Log("パーティクル強制再生！");
}
else
{
    // 子オブジェクトに ParticleSystem がある場合
    ParticleSystem childPS = effect.GetComponentInChildren<ParticleSystem>();
    if (childPS != null)
    {
        childPS.Play();
        Debug.Log("子オブジェクトのパーティクルを再生！");
    }
    else
    {
        Debug.LogWarning("ParticleSystem が見つかりません！");
    }
}
    }
//レベルアップ効果音
    if (levelUpSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(levelUpSound);
    }

    Debug.Log("レベルアップ！");
}
}