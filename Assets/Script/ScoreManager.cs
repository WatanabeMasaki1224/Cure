using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;

    public void OnRepairPointStateChanged(RepairPoint rp, bool isFullyRepaired)
    {
        if (isFullyRepaired)
        {
            score += 100; // 修復成功の得点例     
        }
        else
        {
            score -= 50; // 壊されたペナルティ例
        }
    }
}
