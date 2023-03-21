using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Settings")]
    public GameObject[] _prefabEnemies;
    public float _enemySpawnPerSecond = 0.5f;
    public float _enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] _weaponDefinitions;
    public GameObject _prefabPowerUp;
    public WeaponType[] _powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, 
                                                                WeaponType.spread, WeaponType.spread };

    private BoundsCheck _bndCkeck;

    private void Awake()
    {
        S = this;
        _bndCkeck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / _enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in _weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, _prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(_prefabEnemies[index]);

        float enemyPadding = _enemyDefaultPadding;
        BoundsCheck goBndCheck = go.GetComponent<BoundsCheck>();
        if (goBndCheck != null)
        {
            enemyPadding = Mathf.Abs(goBndCheck._radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -_bndCkeck._camWidth + enemyPadding;
        float xMax = _bndCkeck._camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = _bndCkeck._camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / _enemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy e)
    {
        if(Random.value <= e._powerUpDropChance)
        {
            int index = Random.Range(0, _powerUpFrequency.Length);
            WeaponType puType = _powerUpFrequency[index];

            GameObject go = Instantiate(_prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            pu.transform.position = e.transform.position;
        }
    }

    /// <summary>
    /// A static function that returns a WeaponDefinition from 
    /// the protected WEEP_DICT field of the Main class.
    /// </summary>
    /// <returns>
    /// An instance of WeaponDefinition or, if there is no such 
    /// definition for the specified Weapon Type, returns a new
    /// instance of WeaponDefinition with the type none.
    /// </returns>
    /// <param name="wt">
    /// WeaponType, for which you want to get the WeaponDefinition
    /// </param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        }

        return new WeaponDefinition();
    } 


}
