using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))] //Zorgt ervoor dat het script alleen maar werkt als er een characterController is
public class CharacterControllerBehaviour : MonoBehaviour {
#pragma warning disable 649
    [SerializeField] //Serialized fields kunnen in de editor gezien worden
    private Transform _absoluteTransform; //of relativeTransform

    private CharacterController _charCtrl;
    private Vector3 _velocity= Physics.gravity;
    private Vector3 _inputMovement;
    [SerializeField]
    private float _acceleration = 3.0f; //m/s^2
#pragma warning restore 649

    // Use this for initialization
    void Start () {

        _charCtrl = GetComponent<CharacterController>();                                    //Generic Method (Als de component niet bestaat zal het veel geheugen verbruiken)
       // _charCtrl = (CharacterController)GetComponent(typeof(CharacterController));       //Non-Generic method (will crash if it doesn't work)
       // _charCtrl = GetComponent("CharacterController") as CharacterController;           //Non-Generic method (Will return null if it doesnt work)

        //typeof can tijdens compile time het klassentype teruggeven
        //(CharacterController) en typeof(CharacterController) zijn hetzelfde dus de functie kan returnen

#if DEBUG
        //Conditional, zorgt ervoor dat al dan niet sommige stukken code worden meegenomen of niet
        /*if (_charCtrl == null)
        {
            Debug.LogError("DEPENDENCY ERROR: CharacterControllerBehaviour needs a CharacterControllerComponent");
            
        }*/
        Assert.IsNotNull(_charCtrl, "DEPENDENCY ERROR: CharacterControllerBehaviour needs a CharacterControllerComponent"); //If object is null, assertion fails and error gets sent out
#endif

	}

    private void Update()
    {
        //Input apart van wiskundige berekening houden | Inputs in update() doen omdat het een keer per frame wordt gedaan
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Input vertalen tov een ander gameobject (bv Camera) | extra veld aanmaken om movement te kunnen vertalen
    }


    //Gravity changes should be done in FixedUpdate so it isn't dependent on FPS
    //Abstractie-niveaus : kijken naar het huis of de bakstenen van het huis | huis beschrijven met ramen en deuren of beschrijven hoe de ramen en deuren gemaakt zijn en bij elkaar zitten

    // Update is called once per frame
    void FixedUpdate () {

        Grounded();

        Falling();

        ApplyMovement();

        DoMovement();
	}

    private void Grounded()
    {
        if (_charCtrl.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity); //Project() projects a vector onto another vector | With dotproduct
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
}
