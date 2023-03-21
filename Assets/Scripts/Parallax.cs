using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Settings")]
    public GameObject _poi;
    public GameObject[] _panels;
    public float _scrollSpeed = -30f;
    public float _motionMult = 0.25f; // the degree of response of the panels to the movement of the player's ship

    private float _panelHt;
    private float _depth;

    void Start()
    {
        _panelHt = _panels[0].transform.localScale.y;
        _depth = _panels[0].transform.position.z;

        _panels[0].transform.position = new Vector3(0, 0, _depth);
        _panels[1].transform.position = new Vector3(0, _panelHt, _depth);
    }

    void Update()
    {
        float tY, tX = 0;
        tY = Easing();

        if(_poi != null)
        {
            tX = -_poi.transform.position.x * _motionMult;
        }

        _panels[0].transform.position = new Vector3(tX, tY, _depth);
        if (tY >= 0)
        {
            _panels[1].transform.position = new Vector3(tX, tY - _panelHt, _depth);
        }
        else
        {
            _panels[1].transform.position = new Vector3(tX, tY + _panelHt, _depth);
        }
    }

    float Easing()
    {
        return Time.time * _scrollSpeed % _panelHt + (_panelHt * 0.5f);
    }
}
