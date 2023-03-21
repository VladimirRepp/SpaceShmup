using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 _rotMinMax = new Vector2(15, 90);
    public Vector2 _driftMinMax = new Vector2(0.25f, 2);
    public float _lifeTime = 6f;
    public float _fadeTime = 4f;

    [Header("Parameters")]
    public WeaponType _type;
    public GameObject _cube;
    public TextMesh _letter;
    public Vector3 _rotPerSecond;
    public float _birthTime;

    private Rigidbody _rigid;
    private BoundsCheck _bndCheck;
    private Renderer _cubeRend;

    private void Awake()
    {
        _cube = transform.Find("Cube").gameObject;
        _letter = GetComponent<TextMesh>();
        _rigid = GetComponent<Rigidbody>();
        _bndCheck = GetComponent<BoundsCheck>();
        _cubeRend = _cube.GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize(); // set vector length 1
        vel *= Random.Range(_driftMinMax.x, _driftMinMax.y);
        _rigid.velocity = vel;

        transform.rotation = Quaternion.identity;

        _rotPerSecond = new Vector3(Random.Range(_rotMinMax.x, _rotMinMax.y),
           Random.Range(_rotMinMax.x, _rotMinMax.y),
           Random.Range(_rotMinMax.x, _rotMinMax.y));

        _birthTime = Time.time;
    }

    private void Update()
    {
        _cube.transform.rotation = Quaternion.Euler(_rotPerSecond * Time.time);

        float u = Easing();

        if(u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if(u > 0)
        {
            Color c = _cubeRend.material.color;
            c.a = 1f - u;
            _cubeRend.material.color = c;

            // The letter dissolves more slowly than the cube
            c = _letter.color;
            c.a = 1f - (u * 0.5f);
            _letter.color = c;
        }

        if (!_bndCheck._isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        _cubeRend.material.color = def.color;
        _letter.text = def.letter;
        _type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }

    float Easing()
    {
        return (Time.time - (_birthTime + _lifeTime)) / _fadeTime;
    }
}
