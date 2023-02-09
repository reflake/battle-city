using UnityEngine;

using Zenject;

public class Tank : MonoBehaviour, IDestructible
{
    [SerializeField] private float speed;
    [SerializeField, Range(1, 10)] private int hp;
    [Space]
    [SerializeField] private Bullet bulletPrefab;

    [Inject] private readonly SpriteRenderer _spriteRenderer = null;
    [Inject] private readonly Rigidbody2D _rig = null;
    [Inject] private readonly Collider2D _collider = null;
    
    private Direction _currentDirection = Direction.None;
    public bool Alive { get; private set; } = true;

    public void Shoot(Direction shootDirection)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        
        Face(shootDirection);
        
        bullet.Shoot(shootDirection, _collider);
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
        hp--;
        
        if (hp < 1)
        {
            Alive = false;

            Destroy(gameObject);
        }
    }
}
