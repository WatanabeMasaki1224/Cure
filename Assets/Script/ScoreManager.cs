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
            score += 100; // �C�������̓��_��     
        }
        else
        {
            score -= 50; // �󂳂ꂽ�y�i���e�B��
        }
    }
}
