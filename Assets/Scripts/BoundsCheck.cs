using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prevents the game object from going off the screen
/// IMPORTANT: Only works with the Main Camera orthographic camera in P[0, 0, 0]
/// </summary>

public class BoundsCheck : MonoBehaviour
{
    [Header("Settings")]
    public float _radius = 1f;
    public bool _keepOnScreen = true;
    public bool _drawGizmos = false;

    [Header("Parameters")]
    public bool _isOnScreen = true;
    public float _camWidth;
    public float _camHeight;

    [HideInInspector]
    public bool _offRight, _offLeft, _offUp, _offDown;

    private void Awake()
    {
        _camHeight = Camera.main.orthographicSize;
        _camWidth = _camHeight * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        _isOnScreen = true;
        _offDown = _offLeft = _offRight = _offUp = false;

        if (pos.x > _camWidth - _radius)
        {
            pos.x = _camWidth - _radius;
            _offRight = true;
        }
        if(pos.x < -_camWidth + _radius)
        {
            pos.x = -_camWidth + _radius;
            _offLeft = true;
        }

        if(pos.y > _camHeight - _radius)
        {
            pos.y = _camHeight - _radius;
            _offUp = true;
        }
        if(pos.y < -_camHeight + _radius)
        {
            pos.y = -_camHeight + _radius;
            _offDown = true;
        }

        _isOnScreen = !(_offDown || _offLeft || _offRight || _offUp);

        if(_keepOnScreen && !_isOnScreen)
        {
            transform.position = pos;
            _isOnScreen = false;
            _offDown = _offLeft = _offRight = _offUp = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!_drawGizmos)
            return;

        Vector3 boundSize = new Vector3(_camWidth * 2, _camHeight * 2, 0.1f);
        Color c = new Color(0.9f, 0.75f, 0.2f, 0.5f);

        Gizmos.color = c;
        Gizmos.DrawCube(Vector3.zero, boundSize);
    }
}
