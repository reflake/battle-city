using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rig = null;
    [SerializeField] private Collider2D collider = null;
    [SerializeField] private float speed = 187f;
    
    private Vector2 _linearVelocity;

    public void Setup(Direction direction, Collider2D ignoreCollider)
    {
        _linearVelocity = VectorForDirection(direction) * speed;
        
        Physics2D.IgnoreCollision(ignoreCollider, collider);
    }
    
    void FixedUpdate()
    {
        rig.velocity = _linearVelocity * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
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
