using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{

    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _positionReference;
    private bool _isRotating;
    private bool _isMoving;
    private bool _mouseOn = false;


    void Start()
    {
        _sensitivity = 0.4f;
    }

    void Update()
    {
        UpdateMouseStatus();
        // offset
        Vector3 _mouseOffset = (Input.mousePosition - _mouseReference) * _sensitivity;
        if (_isRotating)
        {
            Vector3 v;
            v.x = 0; v.y = 0;  v.z = _mouseOffset.x;
            transform.eulerAngles = v;
            // store mouse
            //_mouseReference = Input.mousePosition;
        }
        if (_isMoving)
        {
            
        }
    }

    private void OnMouseEnter()
    {
        _mouseOn = true;
    }

    private void OnMouseExit()
    {
        _mouseOn = false;
    }

    void UpdateMouseStatus()
    {
        if (_mouseOn & (Input.GetMouseButtonDown(0) | Input.GetMouseButtonDown(1)))
        {
            _isRotating = Input.GetMouseButton(0);
            _isMoving = Input.GetMouseButton(1);

            _mouseReference = Input.mousePosition;
            _positionReference = transform.position;
        }
        if ((_isMoving & Input.GetMouseButtonUp(1))|(_isRotating&Input.GetMouseButtonUp(0)))
        {
            _isRotating = Input.GetMouseButton(0);
            _isMoving = Input.GetMouseButton(1);
        }
    }

}