using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck _bndCkeck;
    private Renderer _rend;

    [Header("Parameters")]
    public Rigidbody _rigid;
    [SerializeField]
    private WeaponType _type;

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

    private void Awake()
    {
        _bndCkeck = GetComponent<BoundsCheck>();
        _rend = GetComponent<Renderer>();
        _rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_bndCkeck._offUp)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Changing the hidden _type field and setting the color
    /// of the projectile as defined in the WeaponDefinition
    /// </summary>
    /// <param name="eType">
    /// The type WeaponType of weapon used
    /// </param>
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        _rend.material.color = def.projectileColor;
    }
}
