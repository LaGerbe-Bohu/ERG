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
    public Transform transformBias;
    
    [Header("Movement Values")]
    public float acceleration;
    public float maxSpeed;
    public float GravityScale;
    // some private var
    private Vector2 direction;
    
    void Update()
    {
        direction = playerInput.actions["Deplacement"].ReadValue<Vector2>(); // get direction input


        OrientCharacter();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }


    void OrientCharacter()
    {
        var up = rigidBody.transform.up;
        Vector3 forward = Vector3.ProjectOnPlane(cameraPlayer.transform.forward, up);
        
        transformBias.transform.LookAt(this.rigidBody.transform.position + forward*2);
        
    }
    
    void MoveCharacter()
    {
        // move character
        var up = cameraPlayer.transform.up;
        Vector3 forward = transformBias.transform.forward;
        Vector3 right =  transformBias.transform.right;
        
        rigidBody.AddForce(forward* direction.y * acceleration,ForceMode.Acceleration);
        rigidBody.AddForce(right * direction.x * acceleration,ForceMode.Acceleration);
        
        rigidBody.AddForce(-Vector3.up*GravityScale,ForceMode.Acceleration);


        var velocity = rigidBody.velocity;
        float tmpY = velocity.y;
        
        // normalise velocity speed
        Vector3 tmpVelo;
        tmpVelo = Vector3.ClampMagnitude(velocity, maxSpeed);
        tmpVelo = new Vector3(tmpVelo.x, tmpY, tmpVelo.z);
        rigidBody.velocity = tmpVelo;

       

    }
    
    
    
}
