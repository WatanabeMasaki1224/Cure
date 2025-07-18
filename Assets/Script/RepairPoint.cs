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

    //�_���[�W�󂯂��Ƃ��̏���
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
    //�Đ����@�ŏC��
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
            // �C����Ԃ��ς�����̂ŃC�x���g�ʒm
            OnRepairStateChanged?.Invoke(this, isFullyRepaired);
        }
    }

    private void UpdateVisuals()
    {
        // ��FHP�ɉ����ĐF��ς�����A�G�t�F�N�g��ς���Ȃ�
        float t = (float)currentHP / maxHp;
        // �����Ɍ����ڍX�V����
        Color color = Color.Lerp(Color.red,Color.green,t);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Debug.Log($"[UpdateVisuals] HP: {currentHP}, Color: {color}");
        if (sr != null )
        {
            sr.color = color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer ��������܂���");
        }
    }
    public bool IsAttackable()
    {
        return currentHP > 0;
    }
}
