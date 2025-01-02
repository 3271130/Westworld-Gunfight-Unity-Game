using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiamondManager : MonoBehaviour
{
    public GameObject Diamond; // 钻石 Prefab
    public int numberOfDiamonds = 50; // 生成钻石的数量
    public int DiamondLevelUp = 4; // 升级需要的钻石数量
    public Transform[] spawnPoints; // 可用的生成点
    public TextMeshProUGUI diamondCountText; // 显示钻石数量的 UI Text
    private ydhPlayerCharacter ydhPlayerCharacter; // 玩家角色引用
    public bool isReward = false;

    private int playerDiamondCount = 0;

    public string[] rewards = {
        "You have won the bananaShell. It has no power at all!",
        "You have won the tntShell. It has larger attack radius!",
        "You have won the knifeShell. It hurts badly!"
    }; // 奖项信息
    public TextMeshProUGUI rewardText; // 显示奖项信息的 UI Text

    void SpawnDiamonds()
    {
        // 如果没有设置 spawnPoints 或没有 Diamond Prefab，则跳过
        if (Diamond == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Diamond Prefab 或 spawnPoints 未设置！");
            return;
        }

        // 生成 50 个钻石
        for (int i = 0; i < numberOfDiamonds; i++)
        {
            // 随机选择一个生成点
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            // 随机偏移生成点的位置，使得钻石的分布更广
            Vector3 spawnPosition = spawnPoint.position + new Vector3(
                Random.Range(-5f, 5f),  // X轴偏移范围
                0f,                    // Y轴不偏移
                Random.Range(-5f, 5f)   // Z轴偏移范围
            );
            // 在随机位置生成钻石
            Instantiate(Diamond, spawnPosition, Quaternion.identity);
        }
    }

    void Start()
    {
        ydhPlayerCharacter = FindObjectOfType<ydhPlayerCharacter>(); // 获取玩家角色
        SpawnDiamonds(); // 游戏开始时生成 50 个钻石
        UpdateDiamondCountUI(); // 初始化 UI 显示
    }

    public void CollectDiamond()
    {
        playerDiamondCount++;
        UpdateDiamondCountUI(); // 每次收集钻石后更新 UI
        if (playerDiamondCount % DiamondLevelUp == 0)
        {
            Debug.Log("抽奖系统");
            isReward = true;
            // 开始替换子弹的协程
            ydhPlayerCharacter.ReplaceShellCoroutine(); // 调用 PlayerCharacter 中的新方法
        }
    }

    public void ShowReward(int index)
    {
        rewardText.text = rewards[index]; // 更新 UI 文本
        rewardText.gameObject.SetActive(true); // 显示奖项信息
        StartCoroutine(HideRewardCoroutine()); // 启动协程隐藏奖项信息
    }

    public IEnumerator HideRewardCoroutine()
    {
        yield return new WaitForSeconds(5f); // 等待5秒
        rewardText.gameObject.SetActive(false); // 隐藏奖项信息
    }

    void UpdateDiamondCountUI()
    {
        int displayCount = 0;
        if (playerDiamondCount % DiamondLevelUp != 0)
        {
            displayCount = playerDiamondCount % DiamondLevelUp; // 计算当前显示的钻石数量
        }
        else
        {
            displayCount = DiamondLevelUp; // 如果是20的倍数，显示 20
        }   
        diamondCountText.text = "Diamond Number: " + displayCount.ToString(); // 更新 UI
    }

    
}
