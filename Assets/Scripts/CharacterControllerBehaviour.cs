using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))] //Zorgt ervoor dat het script alleen maar werkt als er een characterController is
public class CharacterControllerBehaviour : MonoBehaviour {

    private CharacterController _charCtrl;
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
    // Update is called once per frame
    void Update () {

	}


}
