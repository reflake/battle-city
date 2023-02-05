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
        _linearVelocity = direction.ToVector() * speed;
        
        Physics2D.IgnoreCollision(ignoreCollider, collider);
    }
    
    void FixedUpdate()
    {
        rig.velocity = _linearVelocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Base"))
        {
            other.GetComponent<Base>().Kill();
        }
        
        Destroy(gameObject);
    }
}
