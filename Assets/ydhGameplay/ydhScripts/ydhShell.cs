using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ydhShell : MonoBehaviour //炮弹的行为脚本
{
    public float lifeTimeMax = 2f; //设置炮弹的最大生命周期，默认值为2秒。当达到这个时间，炮弹会自动销毁。
    public AudioSource explosionAudioSource; //音频源。
    public ParticleSystem explosionParticles; //用于显示爆炸粒子特效。
    public float explosionRadius; //定义爆炸的影响范围半径，影响范围内的对象会受到伤害。
    public float explosionForce = 1000f; //爆炸的冲击力，用于添加爆炸产生的物理效果（如推开物体等）。
    public float damageMax = 100f; //爆炸伤害的最大值，当目标对象距离爆炸中心越近，伤害越大。
    public LayerMask damageMask; //用于限定爆炸影响的对象范围，只影响特定层级的对象。
    public bool isRotate = false; //指示炮弹是否需要旋转（例如在飞行中滚动的视觉效果）。

    void Start ()
    {
        Destroy(gameObject, lifeTimeMax); //在 lifeTimeMax 秒后销毁炮弹对象，避免无限存在。
        if(isRotate)
        {
            GetComponent<Rigidbody>().AddTorque(transform.right * 1000); //实现炮弹旋转。
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //在炮弹当前位置生成一个半径为 explosionRadius 的球形区域，检测在 damageMask 层级范围内的所有碰撞体。
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);

        foreach(var collider in colliders) //遍历球形区域内的碰撞体：
        {
            var targetCharacter = collider.GetComponent<ydhPlayerCharacter>();
            if (targetCharacter)
            {
                targetCharacter.TakeDamage(CalculateDamage(collider.transform.position));
            }
            //如果碰撞体附加了 PlayerCharacter 脚本，则调用其 TakeDamage() 方法，传入通过 CalculateDamage 方法计算的伤害值。
        }
        explosionParticles.transform.parent = null;

        // 检查音频源是否启用
        if (explosionAudioSource != null && explosionAudioSource.gameObject.activeInHierarchy)
        {
            explosionAudioSource.enabled = true;
            explosionAudioSource.Play(); // 播放音频
        }
        else
        {
            Debug.LogWarning("Explosion audio source is disabled or not assigned.");
        }

        explosionParticles.Play();

        ParticleSystem.MainModule mainModule = explosionParticles.main;
        Destroy(explosionParticles.gameObject, mainModule.duration);
        Destroy(gameObject); //销毁炮弹对象。
    }


    float CalculateDamage(Vector3 targetPosition) //计算爆炸对目标对象造成的伤害。
    {
        
        var distance = (targetPosition - transform.position).magnitude;
        //距离越小，伤害比例越大
        var damageModify = (explosionRadius - distance) / explosionRadius;
        var damage = damageModify * damageMax;
        return Mathf.Max(2f, damage); //确保伤害值至少为 2，即即使目标位于爆炸边缘，也能受到最低伤害。
    }
}
