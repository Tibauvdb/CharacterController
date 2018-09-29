using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DoubleJump : MonoBehaviour {
    #region Base
#pragma warning disable 649
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

#pragma warning restore 649
    #endregion

    //private bool _jump = false;
    int _jump = 1;
    enum jump { NoJump,FirstJump,SecondJump};
    void Start()
    {
        _charCtrl = GetComponent<CharacterController>();                                    
    }

    private void Update()
    {

        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            Debug.Log(_jump);
        if (Input.GetButtonDown("Jump"))
        {
            if (_jump == (int)jump.NoJump && _charCtrl.isGrounded)
            {
                _jump = (int)(jump.FirstJump);

            }
        }

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
            _velocity -= Vector3.Project(_velocity, Physics.gravity); //Project() projects a vector onto another vector | With dotproduct
            _jump = (int)jump.FirstJump;
        }
    }

    private void Falling()
    {
        if (!_charCtrl.isGrounded)
        {
            _velocity += Physics.gravity * Time.fixedDeltaTime; //Increment so that the velocity keeps increasing
                                                                //deltaTime is werkelijke frametime en fixeddeltatime is physics frametime (in fixed update zijn beide fixedDeltaTime)

        }
    }

    private void ApplyMovement()
    {
        if (_charCtrl.isGrounded)
        {
            Vector3 xzForward = new Vector3(_absoluteTransform.forward.x, 0, _absoluteTransform.forward.z); //Forward on the xz-plane

            xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1)); //Faster version of above

            Quaternion relativeRotation = Quaternion.LookRotation(xzForward);

            Vector3 relativeMovement = relativeRotation * _inputMovement;

            _velocity += relativeMovement * _acceleration * Time.fixedDeltaTime;
        }


        //...
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
                Vector3.ClampMagnitude(_velocity, _MaximumXZVelocity); //Limiteert ook in de y richting

            _velocity = clampedXZVelocity + yVelocity;
        }
    }

    private void ApplyJump()
    {
        if (Input.GetButtonDown("Jump") && (_jump == (int)jump.FirstJump || _jump==(int)jump.SecondJump) ) 
        {

            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);

           if(_jump == (int)jump.SecondJump)
            {
                _jump = (int)jump.NoJump;
                
            }

            if (_jump == (int)jump.FirstJump)
            {
                _jump = (int)jump.SecondJump;
            }

        }
    }

}
