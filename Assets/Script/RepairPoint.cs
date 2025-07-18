using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPoint : MonoBehaviour
{
    public int maxHp = 5;
    private int currentHP;
    public delegate void RepairPointStateChanged(RepairPoint rp, bool isFullyRepaired);
    public event RepairPointStateChanged OnRepairStateChanged;
    private bool isFullyRepaired = false;

    private void Start()
    {
        currentHP = 0;
        UpdateVisuals();
    }

    //ダメージ受けたときの処理
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;  
        }
        CheakRepairStateChanged();
        UpdateVisuals();
    }
    //再生魔法で修復
    public void Repair(int repairAmount)
    {
        currentHP += repairAmount;
        if (currentHP > maxHp)
        {
            currentHP = maxHp;
        }
        CheakRepairStateChanged();
        UpdateVisuals();
    }

    public bool IsFullyRepaired() 
    {
        return isFullyRepaired;
    }

    private void CheakRepairStateChanged()
    {
        bool wasFullyRepaired = isFullyRepaired;
        isFullyRepaired = (currentHP >= maxHp);

        if (wasFullyRepaired != isFullyRepaired)
        {
            // 修復状態が変わったのでイベント通知
            OnRepairStateChanged?.Invoke(this, isFullyRepaired);
        }
    }

    private void UpdateVisuals()
    {
        // 例：HPに応じて色を変えたり、エフェクトを変えるなど
        float t = (float)currentHP / maxHp;
        // ここに見た目更新処理
        Color color = Color.Lerp(Color.red,Color.green,t);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Debug.Log($"[UpdateVisuals] HP: {currentHP}, Color: {color}");
        if (sr != null )
        {
            sr.color = color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer が見つかりません");
        }
    }
    public bool IsAttackable()
    {
        return currentHP > 0;
    }
}
