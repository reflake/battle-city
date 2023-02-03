using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Tank : MonoBehaviour
{
    [SerializeField] private float speed;

    [Inject] private readonly Rigidbody2D _rig = null;

    void FixedUpdate()
    {
        float inputAxisX = Input.GetAxis("Horizontal");
        float inputAxisY = Input.GetAxis("Vertical");

        Vector2 inputVelocity = new Vector2(inputAxisX, inputAxisY) * speed;
        
        _rig.velocity = inputVelocity * Time.fixedDeltaTime;
    }
}
