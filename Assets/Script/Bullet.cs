using UnityEditor.Search;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 2f;
    Vector2 direction;

    public void Init(Vector2 dir)
    {
         direction = dir.normalized;
         Destroy(gameObject,lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
