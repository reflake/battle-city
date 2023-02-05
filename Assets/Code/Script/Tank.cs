using UnityEngine;

using Zenject;

public class Tank : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Bullet bulletPrefab;
    
    [Inject] private readonly Rigidbody2D _rig = null;
    [Inject] private readonly Collider2D _collider = null;
    
    private Direction _currentDirection = Direction.None;
    private bool _alive = true;

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

    public void Kill()
    {
        if (_alive)
        {
            _alive = false;
            
            Destroy(gameObject);
        }
    }
}
