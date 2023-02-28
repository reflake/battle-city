using System;
using System.Linq;
using Common;
using Effects;
using UnityEngine;
using Zenject;

namespace Tanks
{
    public partial class Bullet : MonoBehaviour
    {
        [Inject] readonly EffectManager _effectManager = null;
        
        [field: SerializeField] public Team Team { get; set; }
        
        [SerializeField] AnimationData bulletExplosionEffect;
        [SerializeField] SpriteRenderer spriteRenderer = null;
        [SerializeField] Rigidbody2D rig = null;
        [SerializeField] Collider2D collider = null;
        [SerializeField] float speed = 187f;
    
        Vector2 _vectorDirection;
        float _projectileSpeed;
        Direction _direction;
        int _firePower;
        int _damage;
        BulletHitDelegate _hitCallback;
        RaycastHit2D[] _raycastResults = new RaycastHit2D[16];
        Collider2D _ignoreCollider;

        void Shoot(Team team, Vector3 position, Direction shootDirection, Stats shooterStats,
            Collider2D ignoreCollider2D)
        {
            Team = team;
            transform.position = position;
            rig.position = position;
            
            _firePower = shooterStats.firePower;
            _damage = shooterStats.damageBonus + 1;
            _vectorDirection = shootDirection.ToVector();
            _projectileSpeed = shooterStats.projectileSpeed * speed;
            _direction = shootDirection;
            _ignoreCollider = ignoreCollider2D;

            spriteRenderer.TurnToDirection(shootDirection);

            var linearVelocity = _vectorDirection * _projectileSpeed;
            
            rig.velocity = linearVelocity;
        }

        public void WhenDestroyed(BulletHitDelegate callback)
        {
            _hitCallback = callback;
        }
    
        void FixedUpdate()
        {
            int hitCount = collider.Cast(_vectorDirection, _raycastResults, _projectileSpeed * Time.fixedDeltaTime);

            if (hitCount == 0)
                return;
            
            var validContactPoints = _raycastResults.Take(hitCount)
                .Where(x => x.collider != _ignoreCollider);

            if (Team == Team.Enemy)
            {
                validContactPoints = validContactPoints
                    .Where(x => !(x.transform.TryGetComponent<Tank>(out var tank) && tank.Team == Team.Enemy));
            }

            if (validContactPoints.Count() == 0)
                return;

            var closestImpactPoint = validContactPoints.OrderBy(x => x.distance)
                                                                .First();

            HitSomething(closestImpactPoint.transform, closestImpactPoint.point);

            _effectManager.CreateEffect(closestImpactPoint.point, bulletExplosionEffect);
            _hitCallback?.Invoke();
            
            Dispose();
        }

        void HitSomething(Transform other, Vector2 impactPoint)
        {
            if (!other.CompareTag("Destructible"))
                return;
            
            if (!other.TryGetComponent<IDestructible>(out var destructible))
            {
                throw new Exception("Object tagged as 'Destructible' should have IDestructible component!");
            }
        
            if (destructible.Alive)
            {
                DamageData damageData = new DamageData
                {
                    position = impactPoint,
                    direction = _direction,
                    directionVector = _vectorDirection,
                    firePower = _firePower,
                    damage = _damage,
                };
            
                destructible.TakeDamage(damageData);
            }
        }
    }
}
