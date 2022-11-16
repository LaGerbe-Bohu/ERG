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
    private Vector3 _normalSurface;
    private Vector3 _tangentSurface;
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
        IsGrounded();
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
        if (!_jump || !_isGrounded) return;

        
        rigidBody.AddForce(rigidBody.transform.up*jumpForce,ForceMode.Acceleration);
    }
    
    void IsGrounded()
    {
        RaycastHit hit;

        Vector3 postiion = rigidBody.transform.position + Vector3.up*0.25f;

        if (Physics.SphereCast(postiion, 0.5f, -rigidBody.transform.up, out hit, 1f))
        {
            _normalSurface = hit.normal;
            _tangentSurface = Vector3.Cross( transformBias.transform.right,hit.normal);
            _isGrounded =  true;
            
        }

        _isGrounded = false;
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        var transform1 = rigidBody.transform;
        Gizmos.DrawRay(transform1.position + Vector3.up*0.25f, -transform1.up*1f);
        
        Gizmos.color = Color.red;
        var transform2 = rigidBody.transform;
        Gizmos.DrawWireSphere((transform2.position + Vector3.up*.25f) -transform2.up*1f ,.5f);
        
        Gizmos.color = Color.green;

        var position = this.transform.position;
        Gizmos.DrawRay(position,_normalSurface*10f);
        Gizmos.DrawRay(position,_tangentSurface*10f);
        
    }
}
