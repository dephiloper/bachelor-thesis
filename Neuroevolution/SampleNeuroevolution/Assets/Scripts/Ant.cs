﻿using UnityEngine;

public class Ant : MonoBehaviour
{
    public float VelocityFactor = 1.75f;
    public float RotationStepSize = 3.5f;
    public Brain Brain { private get; set; }
    private Rigidbody2D _rigidbody;
    private double[] _environment;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
    }

    private void Update()
    {
        DrawTentacleSenses();
    }
    
    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.up * VelocityFactor;
        _environment = DetermineEnvironment();
        AdjustRotation();
        Brain.Score++;
    }

    private double[] DetermineEnvironment()
    {
        return new double[]
        {
            ShootRay(transform.up + transform.right),
            ShootRay(transform.up),
            ShootRay(transform.up - transform.right),
            _rigidbody.velocity.x,
            _rigidbody.velocity.y
        };
    }

    private void DrawTentacleSenses()
    {
        if (_environment == null) return;
        
        Debug.DrawRay(transform.position, transform.up + transform.right,
            _environment[0] > 0 ? Color.red : Color.green);
        Debug.DrawRay(transform.position, transform.up, _environment[1] > 0 ? Color.red : Color.green);
        Debug.DrawRay(transform.position, transform.up - transform.right,
            _environment[2] > 0 ? Color.red : Color.green);
    }

    private void AdjustRotation()
    {
        var result = Brain.Think(_environment);

        if (result[0] > result[1])
            _rigidbody.rotation += RotationStepSize;
        else
            _rigidbody.rotation -= RotationStepSize;
    }

    private float ShootRay(Vector2 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction, 1f, LayerMask.GetMask("Wall"));
        if (!hit.collider) return -1f;

        return Vector2.Distance(hit.point, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other) => Destroy(gameObject);
}