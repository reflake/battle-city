using UnityEngine;

using Zenject;

public class Tank : MonoBehaviour, IDestructible
{
    [SerializeField] private float speed;
    [SerializeField, Range(1, 10)] private int hp;
    [Space]
    [SerializeField] private Bullet bulletPrefab;
    
    [Inject] private readonly Rigidbody2D _rig = null;
    [Inject] private readonly Collider2D _collider = null;
    
    private Direction _currentDirection = Direction.None;
    public bool Alive { get; private set; } = true;

    public void Shoot(Direction direction)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        
        bullet.Setup(direction, _collider);
    }

    public void SetMoveDirection(Direction newDirection)
    {
        _currentDirection = newDirection;
    }

    void FixedUpdate()
    {
        if (_currentDirection != Direction.None)
        {
            Vector2 inputWishDir = _currentDirection.ToVector();

            _rig.velocity = inputWishDir * speed * Time.fixedDeltaTime;
        }
        else
        {
            _rig.velocity = Vector2.zero;
        }
    }

    public void TakeDamage()
    {
        hp--;
        
        if (hp < 1)
        {
            Alive = false;

            Destroy(gameObject);
        }
    }
}
