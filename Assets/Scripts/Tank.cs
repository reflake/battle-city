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

        Vector2 inputVelocity = new Vector2(inputAxisX, inputAxisY) * speed;
        
        _rig.velocity = inputVelocity * Time.fixedDeltaTime;
    }
}
