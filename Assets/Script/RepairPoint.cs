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
    public void TakeDmage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            CheakRepairAtateChanged();
            UpdateVisuals();
        }
    }
    //�Đ����@�ŏC��
    public void Repair(int repairAmount)
    {
        currentHP += repairAmount;
        if (currentHP > maxHp)
        {
            currentHP = maxHp;
            CheakRepairAtateChanged();
            UpdateVisuals();

        }
    }

    private void CheakRepairAtateChanged()
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
    }
}
