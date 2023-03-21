    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Settings: Enemy_2")]
    public float _sinEccentricity = 0.6f;
    public float _lifeTime = 10f;

    [Header("Parameters: Enemy_2")]
    public Vector3 p0;  //point on the left border
    public Vector3 p1;  //point on the right border
    public float _birthTime;

    private void Start()
    {
        p0 = Vector3.zero;
        p0.x = -_bndCheck._camWidth - _bndCheck._radius;
        p0.y = Random.Range(-_bndCheck._camHeight, _bndCheck._camHeight);

        p1 = Vector3.zero;
        p1.x = _bndCheck._camWidth + _bndCheck._radius;
        p1.y = Random.Range(-_bndCheck._camHeight, _bndCheck._camHeight);

        if(Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        _birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - _birthTime) / _lifeTime;

        if(u > 1)
        {
            Destroy(gameObject);
            return;
        }

        u = Easing(u);
        pos = (1 - u) * p0 + u * p1;
    }

    private float Easing(float u)
    {
        return u + _sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
    }
}
