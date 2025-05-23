using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class MouseInputController : MonoBehaviour
{

    [SerializeField] Transform rotPivot;
    [SerializeField] LayerMask pickUpLayer;
    [SerializeField] float followSpeed;

    [Header("Fall Variables")]
    [SerializeField] float fallSpeed;
    [SerializeField] float fallSpeedAceleration;
    [SerializeField] float fallSpeedRampUpTime;
    private float _origialFallSpeed;

    [Header("Rotation Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float rotateSpeedAceleration;
    [SerializeField] float rotateSpeedRampUpTime;
    private float _origialRotSpeed;
    [Range(0, 50)][SerializeField] float _rangeRot;
    private bool _desacelerate = false;
    [SerializeField] float timerResetVelocity;

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

    [Header("Shadow")]
    [SerializeField] GameObject shadow;
    [SerializeField] SpriteRenderer shadowSprite;
    [SerializeField] Vector3 offset;


    private void Start()
    {
        _leafPosFirstPos = transform.position;
        _randFinalPos = transform.position.y;
        _origialRotSpeed = rotateSpeed;
        _origialFallSpeed = fallSpeed;
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

        shadow.transform.position = new Vector3(transform.position.x, _randFinalPos, 0) + offset;


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
            if (_isFloatingRight)
            {
                _isFloatingRight = false;

            }
            else
            {
                _isFloatingRight = true;
            }
        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            _isClicking = false;
            Color temp = Color.white;
            temp.a = 0;
            shadowSprite.color = temp;
            if (transform.position.y >= 1)
                _randFinalPos = UnityEngine.Random.Range(-3.5f, 1);

        }

        if ((transform.position.y - _randFinalPos > 0.2f) && !_isClicking)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector3(transform.position.x, _randFinalPos, 0), fallSpeed * Time.deltaTime);
            _isFalling = true;
            RotateLeaf();
        }
        else
        {
            _isFalling = false;
            if ((Mathf.Abs(_currentRot) - 0 > 5))
                RotateLeaf();
        }

        if (_isFalling)
        {
            fallSpeed = Mathf.Lerp(fallSpeed, _origialFallSpeed * fallSpeedAceleration, fallSpeedRampUpTime * Time.deltaTime);
        }
        else
        {
            ResetVelocityFall();
        }
    }

    private void RotateLeaf()
    {
        if (_isClicking) return;

        CalculateDirRot();

        MoveLeaf();

        if (_isFloatingRight)
        {
            rotPivot.Rotate(Vector3.forward * (-rotateSpeed * Time.deltaTime));
        }
        else
        {
            rotPivot.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
        }
    }

    private void MoveLeaf()
    {
        if (!_desacelerate)
        {
            rotateSpeed = Mathf.Lerp(rotateSpeed, _origialRotSpeed * rotateSpeedAceleration, rotateSpeedRampUpTime * Time.deltaTime);
            Color tmp = shadowSprite.color;
            tmp.a = Mathf.Lerp(tmp.a, 0.75f, (rotateSpeedRampUpTime / 3) * Time.deltaTime);
            shadowSprite.color = tmp;
        }
        else
        {

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
            ResetVelocityRot();
        }


        if (_currentRot < -_rangeRot)
        {
            _isFloatingRight = false;
            ResetVelocityRot();

        }
    }

    private void ResetVelocityRot()
    {
        rotateSpeed = 0;
        rotateSpeed = _origialRotSpeed;
    }

    private void ResetVelocityFall()
    {
        fallSpeed = _origialFallSpeed;
    }


}
