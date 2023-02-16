using System;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Rigidbody2D rig = null;
    [SerializeField] Collider2D collider = null;
    [SerializeField] float speed = 187f;
    
    Vector2 _linearVelocity;
    Direction _direction;
    int _damage;
    bool _hitSomething = false;
    Action _destroyCallback;

    public void Shoot(Direction direction, float projectileSpeed, int damage, Collider2D ignoreCollider)
    {
        _damage = damage;
        _direction = direction;
        _linearVelocity = direction.ToVector() * projectileSpeed * speed;
        
        Physics2D.IgnoreCollision(ignoreCollider, collider);
        
        spriteRenderer.TurnToDirection(direction);
    }

    public void WhenDestroyed(Action callback)
    {
        _destroyCallback = callback;
    }
    
    void FixedUpdate()
    {
        rig.velocity = _linearVelocity * Time.fixedDeltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hitSomething)
            return;
        
        if (other.gameObject.CompareTag("Destructible"))
        {
            if (!other.TryGetComponent<IDestructible>(out var destructible))
            {
                throw new Exception("Object tagged as 'Destructible' should have IDestructible component!");
            }
            
            if (destructible.Alive)
            {
                DamageData damageData = new DamageData
                {
                    position = transform.position + transform.forward * .2f,
                    direction = _direction,
                    damage = _damage,
                    strength = 1,
                };
                
                destructible.TakeDamage(damageData);
            }
        }

        _hitSomething = true;
        _destroyCallback?.Invoke();
        Destroy(gameObject);
    }
}
