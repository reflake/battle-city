using System;
using Common;
using UnityEngine;

namespace Tanks
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer = null;
        [SerializeField] Rigidbody2D rig = null;
        [SerializeField] Collider2D collider = null;
        [SerializeField] float speed = 187f;
    
        Vector2 _linearVelocity;
        Direction _direction;
        int _firePower;
        int _damage;
        bool _hitSomething = false;
        Action _destroyCallback;
        ContactPoint2D[] _contactPoint2Ds = new ContactPoint2D[16];

        public void Shoot(Direction direction, float projectileSpeed, int firePower, int damage, Collider2D ignoreCollider)
        {
            _firePower = firePower;
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
            rig.velocity = _linearVelocity;
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
                    Vector3 impactPoint = transform.position;
                    Vector3 penetrationOffset = Vector3.zero;
                
                    switch (_direction)
                    {
                        case Direction.North:
                            penetrationOffset.y = other.bounds.min.y - impactPoint.y;
                            break;
                        case Direction.South:
                            penetrationOffset.y = other.bounds.max.y - impactPoint.y;
                            break;
                        case Direction.East:
                            penetrationOffset.x = other.bounds.min.x - impactPoint.x;
                            break;
                        case Direction.West:
                            penetrationOffset.x = other.bounds.max.x - impactPoint.x;
                            break;
                    }
                
                    impactPoint += penetrationOffset;
                
                    DamageData damageData = new DamageData
                    {
                        position = impactPoint,
                        direction = _direction,
                        firePower = _firePower,
                        damage = _damage,
                    };
                
                    destructible.TakeDamage(damageData);
                }
            }

            _hitSomething = true;
            _destroyCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
