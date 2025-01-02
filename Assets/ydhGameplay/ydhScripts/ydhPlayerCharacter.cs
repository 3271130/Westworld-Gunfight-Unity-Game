using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ydhPlayerCharacter : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public ParticleSystem explosionParticles;

    public Rigidbody shell; // 子弹
    public Transform muzzle;

    public float launchForce = 10; //冲力，作用力

    public float health;
    float healthMax;
    bool isAlive;

    public Slider healthSlider;                             
    public Image healthFillImage;                         
    public Color healthColorFull = Color.green;
    public Color HealthColorNull = Color.red;

    CharacterController cc;

    bool attacking = false;
    public float attackTime;

    Animator animator;

    public Rigidbody originalShell; // 保存原来的子弹
    public bool canReplaceShell = true; // 控制是否可以替换子弹

    public Rigidbody[] shellPrefabs; // 保存所有子弹的引用
    bool hasReplacedShell = false; // 新增标志位

    void Start ()
    {
        animator = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();
        healthMax = health;
        isAlive = true;
        RefreshHealthHUD();
        explosionParticles.gameObject.SetActive(false);
        originalShell = shell; // 初始化时保存原来的子弹
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        RefreshHealthHUD();
        if (health <= 0f && isAlive)
        {
            Death();
        }
    }

    public void RefreshHealthHUD()
    {
        healthSlider.value = health;
        healthFillImage.color = Color.Lerp(HealthColorNull, healthColorFull, health / healthMax);
    }

    public void Death()
    {
        isAlive = false;
        explosionParticles.transform.parent = null;
        explosionParticles.gameObject.SetActive(true);
        ParticleSystem.MainModule mainModule = explosionParticles.main;
        Destroy(explosionParticles.gameObject, mainModule.duration);
        gameObject.SetActive(false);

    }

    public void Move(Vector3 v)
    {
        if (!isAlive) return;
        if (attacking) return;
        Vector3 movement = v * speed;
        cc.SimpleMove(movement);
        if(animator)
        {
            animator.SetFloat("Speed", cc.velocity.magnitude);
        }
    }

    public void Rotate(Vector3 lookDir)
    {
        var targetPos = transform.position + lookDir;
        var characterPos = transform.position;

        //去除Y轴影响
        characterPos.y = 0;
        targetPos.y = 0;
        //角色面朝目标的向量
        Vector3 faceToDir = targetPos - characterPos;
        //角色面朝目标方向的四元数
        Quaternion faceToQuat = Quaternion.LookRotation(faceToDir);
        //球面插值
        Quaternion slerp = Quaternion.Slerp(transform.rotation, faceToQuat, turnSpeed * Time.deltaTime);

        transform.rotation = slerp;
    }

    public void Fire()
    {
        if (!isAlive) return;
        if (attacking) return;

        if (shell == null)
        {
            Debug.LogError("Shell is null!");
            return;
        }

        // 实例化当前设置的子弹
        GameObject shellObj = Instantiate(shell.gameObject, muzzle.position, muzzle.rotation);
        Rigidbody shellRb = shellObj.GetComponent<Rigidbody>();
        
        if (shellRb != null)
        {
            shellRb.linearVelocity = launchForce * muzzle.forward;
        }

        ydhShell shellScript = shellObj.GetComponent<ydhShell>();
        if (shellScript != null && shellScript.explosionAudioSource != null)
        {
            shellScript.explosionAudioSource.Play();
        }

        if (animator)
        {
            animator.SetTrigger("Attack");
        }

        attacking = true;
        Invoke("RefreshAttack", attackTime);

        DiamondManager diamondManager = FindObjectOfType<DiamondManager>();
        if (canReplaceShell)
        {
            if (diamondManager.isReward) // 如果钻石管理器中 isReward 为 true
            {   
                if (!hasReplacedShell)
                {
                    StartCoroutine(ReplaceShellCoroutine());
                    hasReplacedShell = true;
                }
        }
        }
    }

    public IEnumerator ReplaceShellCoroutine()
    {
        // 随机选择一种新的子弹
        int randomIndex = Random.Range(0, shellPrefabs.Length);
        Rigidbody newShell = shellPrefabs[randomIndex];
        DiamondManager diamondManager = FindObjectOfType<DiamondManager>();
        diamondManager.ShowReward(randomIndex); // 新增调用

        // 替换玩家的子弹
        originalShell = shell; // 保存原来的子弹
        shell = newShell; // 替换为新子弹

        Debug.Log("Replaced shell with: " + newShell.name); // 调试信息

        yield return new WaitForSeconds(20f); // 等待20秒

        // 恢复原来的子弹
        shell = originalShell; // 恢复原来的子弹
        Debug.Log("Restored original shell: " + originalShell.name); // 调试信息

        hasReplacedShell = false; // 重置标志位
    }

    void RefreshAttack()
    {
        attacking = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
