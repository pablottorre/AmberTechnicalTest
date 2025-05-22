using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour
{

    [SerializeField] Transform rotPivot;
    [SerializeField] LayerMask pickUpLayer;
    [SerializeField] float followSpeed;
    [SerializeField] float fallSpeed;
    [SerializeField] float rotateSpeed;
    [Range(0, 50)][SerializeField] float _rangeRot;
    private bool _isOnMouse;
    private bool _isClicking;
    private Vector3 _mousePos;
    private Vector3 _mousePosFirstPos;
    private Vector3 _leafPosFirstPos;
    private float _randFinalPos;
    private float _currentRot;
    private bool _isMovingRight;
    private bool _isFloatingRight = true;
    private bool _isFalling;


    private void Start()
    {
        _leafPosFirstPos = transform.position;
        _randFinalPos = transform.position.y;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, pickUpLayer);
        if (hit)
        {
            _isOnMouse = true;
        }
        else
        {
            _isOnMouse = false;
        }

        if (Input.GetMouseButton(0) == true && _isClicking)
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_mousePos != _mousePosFirstPos)
            {
                Vector3 dir = _mousePos - _mousePosFirstPos;
                transform.position = Vector2.Lerp(transform.position, dir + _leafPosFirstPos, followSpeed * Time.deltaTime);
            }
        }

        else if (Input.GetMouseButtonDown(0) == true && _isOnMouse)
        {
            _mousePosFirstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _leafPosFirstPos = transform.position;
            _isClicking = true;
        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            _isClicking = false;
            if (transform.position.y >= 1)
                _randFinalPos = Random.Range(-4, 1);

        }

        if ((transform.position.y - _randFinalPos! > 0.2f) && !_isClicking)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector3(transform.position.x, _randFinalPos, 0), fallSpeed * Time.deltaTime);
            _isFalling = true;
            RotateLeaf();
            //transform.position = new Vector3(transform.position.x, _randFinalPos, 0);
        }
        else
        {
            _isFalling = false;
            if ((Mathf.Abs(_currentRot) - 0 > 5))
                RotateLeaf();
        }

    }

    private void RotateLeaf()
    {
        CalculateDirRot();

        if (_isFloatingRight)
        {
            rotPivot.Rotate(Vector3.forward * (-rotateSpeed * Time.deltaTime));
        }
        else
        {
            rotPivot.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
        }
    }

    private void CalculateDirRot()
    {
        _currentRot = rotPivot.rotation.eulerAngles.z;
        if (_currentRot > 180f)
            _currentRot -= 360f;

        if (_currentRot > _rangeRot)
        {
            _isFloatingRight = true;
        }

        if (_currentRot < -_rangeRot)
        {
            _isFloatingRight = false;
        }
    }
}
