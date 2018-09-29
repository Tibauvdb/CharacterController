using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JumpIncrease : MonoBehaviour {
#pragma warning disable 649

    #region BaseScript
    [SerializeField] //Serialized fields kunnen in de editor gezien worden
    private Transform _absoluteTransform; //of relativeTransform

    private CharacterController _charCtrl;
    [SerializeField]
    private Vector3 _velocity = Physics.gravity;
    private Vector3 _inputMovement;
    //[SerializeField]
    private float _acceleration = 50.0f; //m/s^2

    // [SerializeField]
    private float _dragOnGround = 10; //[] no units

    [SerializeField]
    private float _MaximumXZVelocity = (30.0f * 1000) / (60 * 60); // [m/s] 30km/h

    [SerializeField]
    private float _jumpHeight = 1;

    private bool _jump = false;

    private float _graceJumpTimer = 1.5f; //Players will have 1.5 seconds to jump after leaving an edge
    //If jump is pressed while in the air, the player will touch the ground and then jump again 
    #endregion

    private float _pressTime;
#pragma warning restore 649

    // Use this for initialization
    void Start()
    {
    _charCtrl = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
         
        }
        if (Input.GetButton("Jump"))
        {
            if (_pressTime < 1)
            {
                _pressTime += Time.deltaTime * 10;
            }
            else { _pressTime = 1; }
        }
        Debug.Log(_pressTime);
    }

    void FixedUpdate()
    {


        Grounded();

        Falling();

        ApplyMovement();

        ApplyDragOnGround();

        LimitxzVelocity();

        ApplyJump();

        DoMovement();

    }

    private void Grounded()
    {
        if (_charCtrl.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
            if (_graceJumpTimer != 1.5f)
            {
                _graceJumpTimer = 1.5f;
            }

        }
    }

    private void Falling()
    {
        if (!_charCtrl.isGrounded)
        {
            _velocity += Physics.gravity * Time.fixedDeltaTime;

            _graceJumpTimer -= Time.fixedDeltaTime;
        }
    }

    private void ApplyMovement()
    {
        if (_charCtrl.isGrounded)
        {
            Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));

            Quaternion relativeRotation = Quaternion.LookRotation(xzForward);

            Vector3 relativeMovement = relativeRotation * _inputMovement;

            _velocity += relativeMovement * _acceleration * Time.fixedDeltaTime;
        }
    }

    private void DoMovement()
    {
        Vector3 displace = _velocity * Time.deltaTime;

        _charCtrl.Move(displace);
    }

    private void ApplyDragOnGround()
    {
        if (_charCtrl.isGrounded)
        {
            _velocity *= (1 - Time.fixedDeltaTime * _dragOnGround);
        }
    }

    private void LimitxzVelocity()
    {
        if (_charCtrl.isGrounded)
        {
            Vector3 yVelocity = Vector3.Scale(_velocity, new Vector3(0, 1, 0));
            Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));

            Vector3 clampedXZVelocity =
                    Vector3.ClampMagnitude(_velocity, _MaximumXZVelocity);

            _velocity = clampedXZVelocity + yVelocity;
        }
    }

    private void ApplyJump()
    {
        if (_jump == true && _graceJumpTimer > 0)
        {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);

            _jump = false;

            _graceJumpTimer = -1f;
            _pressTime = 0;

            if(_jump==false && _graceJumpTimer>0 && !_charCtrl.isGrounded)
                {
                    _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * (_pressTime * 10));
                    _pressTime = 0;
                }

        }
        while (Input.GetButtonDown("Jump") && !_charCtrl.isGrounded)
        {
            _velocity.y += (0.5f * 10);
        }
        if (Input.GetButtonUp("Jump")){
            _pressTime = 0;
        }
    }
}
