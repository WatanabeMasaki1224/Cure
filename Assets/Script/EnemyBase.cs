using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("ステータス")]
    public int maxHp = 3;
    protected int currentHP;
    public int damageToPlayer = 1;
    public int damageToRepair = 1;
    [Header("移動")]
    public float moveSpeed = 1f;

    protected virtual void Start()
    {
        currentHP = maxHp;
    }

    // Update is called once per frame
    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("ダメージ");

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
