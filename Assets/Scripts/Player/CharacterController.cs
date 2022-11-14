using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterController : MonoBehaviour
{
    [Header("Compenents")] 
    public Camera cameraPlayer;
    public Rigidbody rigidBody;
    public PlayerInput playerInput;
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput.actions["Forward"].performed += onForward;
    }

    private void onForward(InputAction.CallbackContext context)
    {   
        Debug.Log(context.ReadValue<float>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
