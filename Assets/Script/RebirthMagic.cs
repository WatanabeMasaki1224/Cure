using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthMagic : MonoBehaviour
{
    public float healRadius = 2f; //魔法の範囲
    public float healInterval = 2f;　//何秒ごとに回復するか
    public int healAmount = 1;   //回復量
    public LayerMask repairLayer;
    private Coroutine healingCoroutine;
    private bool isHealing = false;

    public void StartHealing()
    {
        if (!isHealing)
        {
            healingCoroutine = StartCoroutine(HealingRoutine());
            isHealing = true;
        }
    }

    public void StopHealing()
    {
        StopCoroutine(healingCoroutine);
        isHealing=false;
    }

    private IEnumerator HealingRoutine()
    {
        while (true)
        {
            Debug.Log("回復ループ実行中");

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, healRadius, repairLayer);
            foreach(var hit in hits)
            {
                RepairPoint rp = hit.GetComponent<RepairPoint>();
                if (rp != null && !rp.IsFullyRepaired())
                {
                    rp.Repair(healAmount);
                  
                }
            }

            yield return new WaitForSeconds(healInterval);
        }
    }
        
    

}
