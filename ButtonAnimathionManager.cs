using UnityEngine;

public class ButtonAnimathionManager : MonoBehaviour
{


    public Animator animator; // Animatorを参照する
  
public void OnButtonClick()
    {
        // アニメーションを再生するトリガーを設定
        animator.SetTrigger("Big");
    }
}
