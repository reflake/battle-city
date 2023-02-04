using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class Tank : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Bullet bulletPrefab;
    
    [Inject] private readonly Rigidbody2D _rig = null;
    [Inject] private readonly Collider2D _collider = null;
    
    private Direction _currentDirection = Direction.None;

    void Shoot()
    {
        var forward = Quaternion.Euler(0,0, 90f);
        var bullet = Instantiate(bulletPrefab, transform.position, forward);
        
        bullet.Setup(forward, _collider);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    public void SetMoveDirection(Direction newDirection)
    {
        _currentDirection = newDirection;
    }

    void FixedUpdate()
    {
        if (_currentDirection != Direction.None)
        {
            Vector2 inputWishDir = VectorForDirection(_currentDirection);

            _rig.velocity = inputWishDir * speed * Time.fixedDeltaTime;
        }
        else
        {
            _rig.velocity = Vector2.zero;
        }
    }

    Vector2 VectorForDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return Vector2.up;
            case Direction.South: return Vector2.down;
            case Direction.East: return Vector2.right;
            case Direction.West: return Vector2.left;
        }

        throw new Exception($"No vector for this direction: {direction}");
    }
}
