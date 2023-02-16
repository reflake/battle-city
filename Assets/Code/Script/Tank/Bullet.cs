using System;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Rigidbody2D rig = null;
    [SerializeField] private Collider2D collider = null;
    [SerializeField] private float speed = 187f;
    
    private Vector2 _linearVelocity;
    private Direction _direction;
    private int _damage;
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

    private void OnTriggerEnter2D(Collider2D other)
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
