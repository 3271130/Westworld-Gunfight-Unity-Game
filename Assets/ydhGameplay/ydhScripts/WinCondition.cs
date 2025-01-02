using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WinCondition : MonoBehaviour
{
    public ydhPlayerCharacter playerCharacter; // 玩家角色
    public TextMeshProUGUI winText; // 显示胜利信息的 UI Text
    public TextMeshProUGUI countdownText; // 显示倒计时的 UI Text
    private float survivalTime = 0f; // 存活时间计数
    private float countdownTime = 120f; // 倒计时初始值
    public float WinTime = 120f; // 胜利时间
    public List<GameObject> enemies; // 敌人 prefab 列表

    void Start()
    {
        winText.gameObject.SetActive(false); // 初始化时隐藏胜利信息
        countdownTime = WinTime;
        countdownText.text = countdownTime.ToString("F0"); // 初始化倒计时显示
    }

    void Update()
    {
        if (playerCharacter != null)
        {
            if (playerCharacter.IsAlive()) // 检查玩家是否存活
            {
                survivalTime += Time.deltaTime; // 增加存活时间
                countdownTime -= Time.deltaTime; // 减少倒计时
                countdownText.text = countdownTime.ToString("F0"); // 更新倒计时显示

                if (survivalTime >= WinTime) // 检查是否存活超过120秒
                {
                    WinGame(); // 调用胜利方法
                }
            }
            else
            {
                LoseGame(); // 玩家死亡，调用失败方法
            }
        }

        if (AllEnemiesDefeated()) // 检查所有敌人是否被击败
        {
            WinGame(); // 调用胜利方法
        }
    }

    bool AllEnemiesDefeated()
    {
        foreach (var enemy in enemies)
        {
            var playerCharacter = enemy.GetComponent<ydhPlayerCharacter>(); // 获取 AIController 组件
            if (playerCharacter != null && playerCharacter.IsAlive()) // 如果敌人存活
            {
                return false; // 返回 false
            }
        }
        return true; // 所有敌人都被击败
    }

    void WinGame()
    {
        winText.gameObject.SetActive(true); // 显示胜利信息
        winText.text = "YOU WIN"; // 设置胜利信息文本
        Time.timeScale = 0; // 暂停游戏
    }

    void LoseGame()
    {
        winText.gameObject.SetActive(true); // 显示失败信息
        winText.text = "YOU LOSE"; // 设置失败信息文本
        Time.timeScale = 0; // 暂停游戏
    }
} 