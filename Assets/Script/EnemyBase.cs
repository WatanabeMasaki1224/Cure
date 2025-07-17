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
    [Header("スコア")]
    public int scoreValue = 10;  // この敵を倒したときの得

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
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddEnemyScore(scoreValue);
        }
        Destroy(gameObject);
    }

    protected abstract void Move();

    protected void Update()
    {
        Move();
    }
}
