using System;
using Common;
using Cysharp.Threading.Tasks;
using Effects;
using Tanks;
using UnityEngine;
using Zenject;

namespace Tanks
{
    public class Tank : MonoBehaviour, IDestructible
    {
        [Space] 
        [SerializeField] AnimationData bulletExplosionEffect;
        [SerializeField] Bullet bulletPrefab;
        [field: SerializeField] public Stats Stats { get; set; } = default;

        [Inject] readonly EffectManager _effectManager = null;
        [Inject] readonly SpriteRenderer _spriteRenderer = null;
        [Inject] readonly Rigidbody2D _rig = null;
        [Inject] readonly Collider2D _collider = null;

        public bool Alive { get; private set; } = true;
        public bool Powered { get; set; } = false;

        // Stats
        public ITankSprites SpritesData;

        // Events
        public event TankKilledDelegate OnGetKilled;
        public event TankHitDelegate OnGetHit;

        int _currentHp = 0;
        Direction _currentDirection = Direction.None;
        Vector2 _spawnLocation = Vector2.zero;
        int _bulletsFired = 0;
        bool _spawnSet = false;

        public void Shoot(Direction shootDirection)
        {
            if (!Alive || !gameObject.activeSelf)
                return;
            
            if (_bulletsFired >= Stats.fireRate)
                return;

            const float shootOffset = .4f;
            var bullet = Instantiate(bulletPrefab, transform.position + transform.right * shootOffset, Quaternion.identity);
        
            bullet.Shoot(shootDirection, Stats, _collider);
            bullet.WhenDestroyed(BulletHit);

            Face(shootDirection);

            _bulletsFired++;
        }

        void BulletHit(Vector2 impactPoint)
        {
            _effectManager.CreateEffect(impactPoint, bulletExplosionEffect);
            _bulletsFired--;
        }

        public void SetMoveDirection(Direction newDirection)
        {
            _currentDirection = newDirection;
        
            if (newDirection != Direction.None)
            
                Face(newDirection);
        }

        void FixedUpdate()
        {
            _rig.velocity = Vector2.zero;
            
            if (_currentDirection != Direction.None)
            {
                Vector2 inputWishDir = _currentDirection.ToVector();

                _rig.MovePosition(_rig.position + inputWishDir * Stats.moveSpeed * Time.fixedDeltaTime);
            }
        }

        public void Face(Direction direction)
        {
            _spriteRenderer.TurnToDirection(direction);
        }

        public void TakeDamage(DamageData damageData)
        {
            _currentHp--;
        
            OnGetHit?.Invoke(damageData);
        
            // When less than 1 hp kill tank
            if (_currentHp < 1)
            {
                Alive = false;

                gameObject.SetActive(false);
            
                OnGetKilled?.Invoke();
            }
        }

        public async UniTaskVoid Respawn()
        {
            // TODO: show respawn animation
            // Wait before respawn
            await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.DeltaTime, cancellationToken: this.GetCancellationTokenOnDestroy());

            if (!_spawnSet)
                throw new Exception($"Spawn is not set for the tank {gameObject.name}");
        
            gameObject.SetActive(true);
        
            Alive = true;
        
            _currentHp = Stats.hitPoints;

            Vector3 newPosition = _spawnLocation;

            _spriteRenderer.sprite = SpritesData.NormalSprite;
        
            // We need to keep tank's Z position
            newPosition.z = transform.position.z;
        
            transform.position = newPosition;
        }

        void Update()
        {
            if (Powered)
            {
                const float period = .33f;
                bool flicker = (Time.time % (period * 2f)) > period;

                _spriteRenderer.sprite = flicker ? SpritesData.NormalSprite : SpritesData.PoweredSprite;
            }
        }

        public void SetSpawnPosition(Vector2 spawnPosition)
        {
            _spawnSet = true;
            _spawnLocation = spawnPosition;
        }
    }
}
