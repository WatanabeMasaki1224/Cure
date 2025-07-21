using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Textを使う場合

public class ScoreManager : MonoBehaviour
{
    private int score;
    [SerializeField] private Text scoreText;
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 破棄させない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreText();
        RepairPoint[] allpoint = FindObjectsOfType<RepairPoint>();
        foreach (RepairPoint rp in allpoint)
        {
            rp.OnRepairStateChanged += OnRepairPointStateChanged;
        }
    }

    // シーン切り替え時に、Textが変わったら呼ぶためのメソッド
    public void SetScoreText(Text newScoreText)
    {
        scoreText = newScoreText;
        UpdateScoreText();
        
    }

    public void OnRepairPointStateChanged(RepairPoint rp, bool isFullyRepaired)
    {
        if (isFullyRepaired)
        {
            score += 100; // 修復成功の得点例     
        }
        UpdateScoreText();
    }

    public void RepairPointBroken(RepairPoint rp)
    {
        score -= 50;
        UpdateScoreText();
    }
 
    public void AddEnemyScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

}
