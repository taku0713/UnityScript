using UnityEngine;
using System.Collections.Generic;

public class WaterFlow : MonoBehaviour
{
    public Vector2 flowDirection = new Vector2(1f, 0f); //水流の方向(初期設定は右)
    public float flowStrength = 1f; //水流の強さ

    private void OnTriggerStay2D(Collider2D other) //水流に触れたら
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.AddForce(flowDirection.normalized * flowStrength); //触れたプレイヤーに水流の力を加える
        }
    }

    private void OnTriggerExit2D(Collider2D other) //水流に入った後水流から出たら
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            // 水流の速度をゼロにリセット（または弱める）
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0f, rb.linearVelocity.y * 0f); 
        }
    }
}

