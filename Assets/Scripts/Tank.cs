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

    void FixedUpdate()
    {
        float inputAxisX = Input.GetAxis("Horizontal");
        float inputAxisY = Input.GetAxis("Vertical");

        var newDirection = DirectionByInput(inputAxisX, inputAxisY);
        
        if (newDirection != Direction.None)
        {
            Vector2 inputWishDir = VectorForDirection(newDirection);

            _rig.velocity = inputWishDir * speed * Time.fixedDeltaTime;
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

    Direction DirectionByInput(float inputAxisX, float inputAxisY)
    {
        const float inputEpsilon = 0.05f;
        
        if (Mathf.Abs(inputAxisY) > inputEpsilon)
        {
            return inputAxisY > 0 ? Direction.North : Direction.South;
        }
        else if (Mathf.Abs(inputAxisX) > inputEpsilon)
        {
            return inputAxisX > 0 ? Direction.East : Direction.West;
        }
        else
        {
            return Direction.None;
        }
    }
}
