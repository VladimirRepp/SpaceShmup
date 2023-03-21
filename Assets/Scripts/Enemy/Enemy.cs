using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Settings: Enemy")]
    public float _speed = 10;
    public float _fireRate = 0.3f;
    public float _health = 10;
    public float _score = 100;
    public float _showDamageDuration = .1f;
    public float _powerUpDropChance = .5f;

    [Header("Parameters: Enemy")]
    public Color[] _originalColor;
    public Material[] _materials;
    public bool _showingDamage = false;
    public float _damageDoneTime;
    public bool _notifiedOfDestruction = false;

    protected BoundsCheck _bndCheck;

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    private void Awake()
    {
        _bndCheck = GetComponent<BoundsCheck>();

        _materials = Utils.GetAllMaterials(gameObject);
        _originalColor = new Color[_materials.Length];
        for(int i = 0; i<_materials.Length; i++)
        {
            _originalColor[i] = _materials[i].color;
        }
    }

    private void Update()
    {
        Move();

        if(_showingDamage && Time.time > _damageDoneTime)
        {
            UnShowDamage();
        }

        if(_bndCheck != null && _bndCheck._offDown)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;

        switch (go.tag)
        {
            case "ProjectileHero":
                Projectile p = go.GetComponent<Projectile>();
                if (!_bndCheck._isOnScreen)
                {
                    Destroy(go);
                    break;
                }

                ShowDamage();

                _health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(_health <= 0)
                {
                    if (!_notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    _notifiedOfDestruction = true;

                    Destroy(this.gameObject);
                }

                Destroy(go);
                break;

            default:
                Debug.Log("Enemy hit by non-ProjectileHero: " + go.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach(Material m in _materials)
        {
            m.color = Color.red;
        }

        _showingDamage = true;
        _damageDoneTime = Time.time + _showDamageDuration;
    }

    void UnShowDamage()
    {
        for(int i = 0; i<_materials.Length; i++)
        {
            _materials[i].color = _originalColor[i];
        }
        _showingDamage = false;
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= _speed * Time.deltaTime;
        pos = tempPos;
    }
}
