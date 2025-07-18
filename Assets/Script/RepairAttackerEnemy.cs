using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairAttackerEnemy : EnemyBase
{
    private RepairPoint currentTarget;
    public float detectionInterval = 1.0f; //C•œƒ|ƒCƒ“ƒg‚ÌŒŸ¸•p“x
    public float attackRange = 1.0f; // “G‚ªUŒ‚‚ğŠJn‚Å‚«‚é‹——£
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

        //ˆÚ“®
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        //”ÍˆÍ‚È‚çUŒ‚
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
            Debug.Log("C•œ‰ÓŠ‚ğUŒ‚I");
        }

        // HP‚ª0‚É‚È‚Á‚½‚çƒ^[ƒQƒbƒg‰ğœ
        if (!currentTarget.IsFullyRepaired())
        {
            currentTarget = null;
        }
    }
}
