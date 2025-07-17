using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("�X�e�[�^�X")]
    public int maxHp = 3;
    protected int currentHP;
    public int damageToPlayer = 1;
    public int damageToRepair = 1;
    [Header("�ړ�")]
    public float moveSpeed = 1f;

    protected virtual void Start()
    {
        currentHP = maxHp;
    }

    // Update is called once per frame
    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("�_���[�W");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected abstract void Move();

    protected void Update()
    {
        Move();
    }
}
