using System;
using UnityEngine;

using Zenject;

public class Tank : MonoBehaviour, IDestructible
{
    [SerializeField] private float speed;
    [SerializeField, Range(1, 10)] private int firePower;
    [SerializeField, Range(1, 10)] private int maxHp;
    [Space]
    [SerializeField] private Bullet bulletPrefab;

    [Inject] private readonly SpriteRenderer _spriteRenderer = null;
    [Inject] private readonly Rigidbody2D _rig = null;
    [Inject] private readonly Collider2D _collider = null;
    
    public bool Alive { get; private set; } = true;
    public event TankKilledDelegate OnGetKilled;

    private int _currentHp = 0;
    private Direction _currentDirection = Direction.None;
    private Vector3 _spawnLocation = Vector3.zero;

    private void Awake()
    {
        _spawnLocation = _rig.position;
        
        Respawn();
    }

    public void Shoot(Direction shootDirection)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        
        Face(shootDirection);
        
        bullet.Shoot(shootDirection, firePower, _collider);
    }

    public void SetMoveDirection(Direction newDirection)
    {
        _currentDirection = newDirection;
        
        if (newDirection != Direction.None)
            
            Face(newDirection);
    }

    void FixedUpdate()
    {
        if (_currentDirection != Direction.None)
        {
            Vector2 inputWishDir = _currentDirection.ToVector();

            _rig.MovePosition(_rig.position + inputWishDir * speed * Time.fixedDeltaTime);
        }
    }

    public void Face(Direction direction)
    {
        _spriteRenderer.TurnToDirection(direction);
    }

    public void TakeDamage(DamageData _)
    {
        _currentHp--;
        
        // When less than 1 hp kill tank
        if (_currentHp < 1)
        {
            Alive = false;

            gameObject.SetActive(false);
            
            OnGetKilled?.Invoke();
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        
        Alive = true;
        
        _currentHp = maxHp;

        transform.position = _spawnLocation;
    }
}
