using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MagicBase
{
    protected override void OnHit(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnmyController enemy = col.GetComponent<EnemyContoroller>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

    }


}
