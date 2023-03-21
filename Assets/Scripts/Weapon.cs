using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Parameters")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition _def;
    public GameObject _collar;
    public float _lastShotTime;

    private Renderer _collarRend;

    public WeaponType type
    {
        get
        {
            return _type;
        }
        set
        {
            SetType(value);
        }
    }

    private void Start()
    {
        _collar = transform.Find("Collar").gameObject;
        _collarRend = _collar.GetComponent<Renderer>();

        SetType(_type);

        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        Hero hero = rootGO.GetComponent<Hero>();
        if (hero != null)
        {
            hero._fireDelegate += Fire;
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if(type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        _def = Main.GetWeaponDefinition(_type);
        _collarRend.material.color = _def.color;
        _lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if(Time.time - _lastShotTime < _def.delayBetweenShots)
        {
            return;
        }

        Projectile p;
        Vector3 vel = Vector3.up * _def.velocity;

        if(transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p._rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile();   // Projectile - forvard
                p._rigid.velocity = vel;
                
                p = MakeProjectile();   // Projectile - right
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p._rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile();   // Projectile - left
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p._rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(_def.projectilePrefab);

        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = _collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);

        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        _lastShotTime = Time.time;

        return p;
    }
}

/// <summary>
/// Class WeaponDefinition allows you to customize the 
/// properties of a specific type of weapon. For this 
/// class, Main will store an array of elements of the 
/// type WeaponDefinition.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;               // the letter on the bonus cube
    public Color color = Color.white;   // the color of the barrel and the bonus cube
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;  // for laser
    public float delayBetweenShots = 0;
    public float velocity;              // projectile flight speed
}

public enum WeaponType
{
    none = 0,       // По умолчанию, нет оружия
    blaster,        // Простой бластер 
    spread,         // Веерная пушка
    phaser,         // [Пока не реализовано] Волновой фазер
    missible,       // [Пока не реализовано] Самонаводящиеся ракеты
    laser,          // [Пока не реализовано] Наносит повреждение при долговременном воздействие 
    shield          // Увеличивает shieldLevel
}