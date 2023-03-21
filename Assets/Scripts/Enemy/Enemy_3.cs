using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The trajectory is determined by linear interpolation 
/// of the Bezier curve at more than two points
/// </summary>
public class Enemy_3 : Enemy
{
    [Header("Settings: Enemy_3")]
    public float _lifeTime = 5;

    [Header("Parameters: Enemy_3")]
    public Vector3[] _points;
    public float _birthTime;

    private void Start()
    {
        _points = new Vector3[3];
        _points[0] = pos;

        float xMin = -_bndCheck._camWidth + _bndCheck._radius;
        float xMax = _bndCheck._camWidth - _bndCheck._radius;

        Vector3 v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -_bndCheck._camHeight * Random.Range(2.75f, 2);
        _points[1] = v;

        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        _points[2] = v;

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

        Vector3 p01, p12;
        u = Easing(u);
        p01 = (1 - u) * _points[0] + u * _points[1];
        p12 = (1 - u) * _points[1] + u * _points[2];
        pos = (1 - u) * p01 + u * p12;
    }

    private float Easing(float u)
    {
        return u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
    }
}
