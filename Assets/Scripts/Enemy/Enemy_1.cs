using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Settings: Enemy_1")]
    public float _waveFrequency = 2f;
    public float _waveWidth = 4f;
    public float _waveRotY = 45f;

    private float x0;
    private float _birthTime;

    void Start()
    {
        x0 = pos.x;
        _birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - _birthTime;
        float theta = Mathf.PI * 2 * age - _waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + _waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * _waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
