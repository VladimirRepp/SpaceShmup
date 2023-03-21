using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Settings")]
    public float _rotationsPerSecond = 0.1f;

    [Header("Parametrs")]
    public int _levelShown = 0;

    private Material _mat;
   
    void Start()
    {
        _mat = GetComponent<Renderer>().material;   
    }

    void Update()
    {
        int currentLevel = Mathf.FloorToInt(Hero.S.shieldLevel);

        if(_levelShown != currentLevel)
        {
            _levelShown = currentLevel;
            _mat.mainTextureOffset = new Vector2(0.2f * _levelShown, 0);
        }

        float rZ = -(_rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
