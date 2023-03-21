using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Settings general")]
    public float _speed = 30;
    public float _rollMult = -45;
    public float _pitchMult = -30;
    public float _gameRestartDelay = 2f;
    [SerializeField]
    private int _shieldLevel = 1;
    public Weapon[] _weapons;

    [Header("Settings projectile")]
    public GameObject _projectilePrefab;
    public float _projectileSpeed = 40f;

    private GameObject _lastTriggerGO = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate _fireDelegate;

    public int shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);

            if(_shieldLevel < 0)
            {
                Destroy(this.gameObject);

                Main.S.DelayedRestart(_gameRestartDelay);
            }
        }
    }

    private void Start()
    {
        if(S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        ClearWeapons();
        _weapons[0].SetType(WeaponType.blaster);
    }

    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * _speed * Time.deltaTime;
        pos.y += yAxis * _speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * _pitchMult, xAxis * _pitchMult, 0);

        if (Input.GetAxis("Jump") == 1 && _fireDelegate != null)
        {
            _fireDelegate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootTR = other.gameObject.transform.root;
        GameObject go = rootTR.gameObject;

        if(go == _lastTriggerGO)
        {
            return;
        }

        if(go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if(go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            Debug.Log("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu._type)
        {
            case WeaponType.shield:
                _shieldLevel++;
                break;

            default:
                if(pu._type == _weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();

                    if(w != null)
                    {
                        w.SetType(pu._type);
                    }
                }
                else
                {
                    ClearWeapons();
                    _weapons[0].SetType(pu._type);
                }
                break;
        }

        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i<_weapons.Length; i++)
        {
            if(_weapons[i].type == WeaponType.none)
            {
                return _weapons[i];
            }
        }

        return null;
    }

    void ClearWeapons()
    {
        foreach(Weapon w in _weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    void TempFire()
    {
        GameObject go = Instantiate<GameObject>(_projectilePrefab);
        go.transform.position = this.transform.position;
        Rigidbody rigidBd = go.GetComponent<Rigidbody>();

        Projectile proj = go.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidBd.velocity = Vector3.up * tSpeed;
    }
}
