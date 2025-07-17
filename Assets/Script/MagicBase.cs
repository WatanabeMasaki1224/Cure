using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public abstract class MagicBase : MonoBehaviour
{
    public float magicSpeed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;
    protected Vector2 direction;
    
    public virtual void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // X方向がマイナスなら画像を左右反転
            spriteRenderer.flipX = (dir.x > 0);
        }
        Destroy(gameObject,lifeTime);
    }

    protected virtual void Update()
    {
        transform.Translate(direction * magicSpeed * Time.deltaTime);
    }

    protected abstract void OnHit(Collider2D col);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit(collision);
    }
}
