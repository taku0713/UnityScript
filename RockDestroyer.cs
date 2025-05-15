using UnityEngine;

public class RockDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Rockタグが付いたオブジェクトを削除
        if (collision.CompareTag("Trap"))
        {
            Destroy(collision.gameObject);
        }
    }
}
