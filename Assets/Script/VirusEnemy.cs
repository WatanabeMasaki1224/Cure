using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusEnemy : EnemyBase
{
    public float moveDistance = 3f;
    private Vector2 startPoint;
    private int moveDirection = 1; //êiçsï˚å¸ÇÃêßå‰
    public float detection = 5f;
    private GameObject player;
    private bool isChasing = false;

    protected override void Start()
    {
        base.Start();
        startPoint = transform.position;
    }

    protected override void Move()
    {
        if (player == null)
        {
             player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

        }

        float distancePlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distancePlayer < detection)
        {
            isChasing = true;
        }
        else if (distancePlayer > detection * 1.2f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrole();
        }
    }
    private void ChasePlayer()
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * 2f * Time.deltaTime);
        }

    private void Patrole()
    {
        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPoint.x) > moveDistance)
        {
            moveDirection *= -1; // ï˚å¸îΩì]
        }
    }

}
