using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusEnemy : EnemyBase
{
    protected override void Move()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * 2f * Time.deltaTime);
        }
    }
}
