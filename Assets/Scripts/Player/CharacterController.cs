using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


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
    public float jumpForce;

    [Space] 
    public bool airControl = false;
    public float gravityScale;

    // some private var
    private Vector2 _direction;
    private bool _jump;
    private bool _isGrounded;
    void Update()
    {
        // Inputs
        _direction = playerInput.actions["Deplacement"].ReadValue<Vector2>(); // get direction input
        _jump = playerInput.actions["Jump"].IsPressed();
        
        OrientCharacter();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        JumpCharacter();
    }
    
    
    void MoveCharacter()
    {
        // move character
        var biaisTransform = transformBias.transform;
        
        Vector3 forward = biaisTransform.forward;
        Vector3 right =  biaisTransform.right;
        
        rigidBody.AddForce(forward * (_direction.y * acceleration),ForceMode.Acceleration);
        rigidBody.AddForce(right * (_direction.x * acceleration),ForceMode.Acceleration);
        
        rigidBody.AddForce(-Vector3.up*gravityScale,ForceMode.Acceleration);


        var velocity = rigidBody.velocity;
        float tmpY = velocity.y;
        
        // normalise velocity speed
        Vector3 tmpVelo;
        tmpVelo = Vector3.ClampMagnitude(velocity, maxSpeed);
        tmpVelo = new Vector3(tmpVelo.x, tmpY, tmpVelo.z);
        rigidBody.velocity = tmpVelo;

    }



    
    void OrientCharacter()
    {
        var up = rigidBody.transform.up;
        Vector3 forward = Vector3.ProjectOnPlane(cameraPlayer.transform.forward, up);
        
        transformBias.transform.LookAt(this.rigidBody.transform.position + forward*2);
        
    }
    
    private void JumpCharacter()
    {
        if (!this._jump) return;

        bool grounded = IsGrounded();

        if (!grounded) return;
        
        rigidBody.AddForce(rigidBody.transform.up*jumpForce,ForceMode.Acceleration);
        
    }
    
    bool IsGrounded()
    {
        RaycastHit hit;

        Vector3 postiion = rigidBody.transform.position + Vector3.up*0.25f;

        if (Physics.SphereCast(postiion, 0.5f, -rigidBody.transform.up, out hit, 1f))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rigidBody.transform.position + Vector3.up*0.25f, -rigidBody.transform.up*1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((rigidBody.transform.position + Vector3.up*.25f) -rigidBody.transform.up*1f ,.5f);
    }
}
