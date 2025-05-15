using UnityEngine;


public class EatEffect : MonoBehaviour
{
    public ParticleSystem eatEffect;

    public void EatEffectSpown()
    {
        ParticleSystem newParticle = Instantiate(eatEffect);
			// パーティクルの発生場所をこのスクリプトをアタッチしているGameObjectの場所にする。
			newParticle.transform.position = this.transform.position;
			// パーティクルを発生させる。
			newParticle.Play();
    }

   
}
