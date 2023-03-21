using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy_4 creates beyond the upper bound, selects a random
/// point on the screen, and moves to it. When he gets to the
/// place, he chooses another point and continues to move until 
/// the player destroys it
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Settings: Enemy_4")]
    public Part[] _parts;

    private Vector3 p0, p1;
    private float _timeStart;
    private float _duration = 4;

    private void Start()
    {
        p0 = p1 = pos;
        InitMovement();

        Transform t;
        foreach(Part ptr in _parts)
        {
            t = transform.Find(ptr.name);
            if(t != null)
            {
                ptr.go = t.gameObject;
                ptr.mat = ptr.go.GetComponent<Renderer>().material;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!_bndCheck._isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);

                if(prtHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject; ;
                    prtHit = FindPart(goHit);
                }

                //If part protected
                if (prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                //Else
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocatizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);
                }

                bool allDestroyed = true;
                foreach(Part prt in _parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }

                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }

                Destroy(other);
                break;
        }
    }

    void InitMovement()
    {
        p0 = p1;

        float widMinRad = _bndCheck._camWidth - _bndCheck._radius;
        float hgtMinRad = _bndCheck._camHeight - _bndCheck._radius;

        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        _timeStart = Time.time;
    }

    Part FindPart(string n){
        foreach(Part ptr in _parts)
        {
            if(ptr.name == n)
            {
                return ptr;
            }
        }

        return null;
    }

    Part FindPart(GameObject go)
    {
        foreach (Part ptr in _parts)
        {
            if (ptr.go == go)
            {
                return ptr;
            }
        }

        return null;
    }

    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if(prt == null)
        {
            return true;
        }

        return prt.health <= 0;
    }

    void ShowLocatizedDamage(Material m)
    {
        m.color = Color.red;
        _damageDoneTime = Time.time + _showDamageDuration;
        _showingDamage = true;
    }

    public override void Move()
    {
        float u = Easing();

        if(u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = Easing(u);
        pos = (1 - u) * p0 + u * p1;
    }

    public float Easing(float u)
    {
        return 1 - Mathf.Pow(1 - u, 2); 
    }

    public float Easing()
    {
        return (Time.time - _timeStart) / _duration;
    }
}

/// <summary>
/// Ship Parts
/// </summary>
[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}