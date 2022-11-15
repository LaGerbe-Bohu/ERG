using System;
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
   
    
    [Header("Movement Values")]
    public float acceleration;
    
    // some private var

    private Vector2 direction;
    
    
    void Update()
    {
        direction = playerInput.actions["Deplacement"].ReadValue<Vector2>(); // get direction input
    
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }


    
    void MoveCharacter()
    {
        // move character
        rigidBody.AddForce(cameraPlayer.transform.forward * direction.y * acceleration,ForceMode.Acceleration);
        rigidBody.AddForce(cameraPlayer.transform.right * direction.x * acceleration,ForceMode.Acceleration);

     

    }
    
    
    
}
