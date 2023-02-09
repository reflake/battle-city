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

    public void Shoot(Direction direction, Collider2D ignoreCollider)
    {
        _direction = direction;
        _linearVelocity = direction.ToVector() * speed;
        
        Physics2D.IgnoreCollision(ignoreCollider, collider);
        
        spriteRenderer.TurnToDirection(direction);
    }
    
    void FixedUpdate()
    {
        rig.velocity = _linearVelocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
                    position = transform.position + transform.forward * .4f,
                    direction = _direction
                };
                
                destructible.TakeDamage(damageData);
            }
        }
        
        Destroy(gameObject);
    }
}
