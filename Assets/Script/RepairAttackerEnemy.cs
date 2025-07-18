using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairAttackerEnemy : EnemyBase
{
    private RepairPoint currentTarget;
    public float detectionInterval = 1.0f; //�C���|�C���g�̌����p�x
    public float attackRange = 1.0f; // �G���U�����J�n�ł��鋗��
    public float attackCooldown = 2f;
    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(TargetRoutine());
    }

   

    protected override void Move()
    {
        if (currentTarget == null)
        {
            return;
        }

        //�ړ�
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        //�͈͂Ȃ�U��
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            AttackTarget();
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator TargetRoutine()
    {
        while (true)
        {
            FindRepairedPoint();
            yield return new WaitForSeconds(detectionInterval);

        }
    }

    private void FindRepairedPoint()
    {
        RepairPoint[] allPoints = FindObjectsOfType<RepairPoint>();
        float shortest = Mathf.Infinity;
        currentTarget = null;

        foreach (var rp in allPoints)
        {
            if(rp.IsAttackable())
            {
                float distance = Vector3.Distance(transform.position, rp.transform.position);
                if (distance < shortest)
                {
                    shortest = distance;
                    currentTarget = rp;
                }
            }
        }
    }

    private void AttackTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(damageToRepair); 
            Debug.Log("�C���ӏ����U���I");
        }

        // HP��0�ɂȂ�����^�[�Q�b�g����
        if (!currentTarget.IsFullyRepaired())
        {
            currentTarget = null;
        }
    }
}
