using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rig = null;
    [SerializeField] private Collider2D collider = null;
    [SerializeField] private float speed = 187f;

    public void Setup(Quaternion direction, Collider2D ignoreCollider)
    {
        transform.rotation = direction;
        
        Physics2D.IgnoreCollision(ignoreCollider, collider);
    }
    
    void FixedUpdate()
    {
        Vector2 directionVelocity = transform.right * speed;
        
        rig.velocity = directionVelocity * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
