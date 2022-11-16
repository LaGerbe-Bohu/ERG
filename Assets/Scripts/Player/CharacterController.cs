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
        
        Vector3 forward = _tangentSurface;
        Vector3 right =  biaisTransform.right;
        
        
        
        // Movement
        
        Debug.Log(ProjectAngle(_tangentSurface,Vector3.forward,transformBias.right));
        
        rigidBody.AddForce(forward * (_direction.y * acceleration),ForceMode.Acceleration);
        rigidBody.AddForce(right * (_direction.x * acceleration),ForceMode.Acceleration);
        
        // Gravity accentiation
        rigidBody.AddForce(-_normalSurface*gravityScale,ForceMode.Acceleration);


        // Normalise speed
        var velocity = rigidBody.velocity;
        float tmpY = velocity.y;
        
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

        if (Physics.SphereCast(postiion, 0.8f, -rigidBody.transform.up, out hit, 1f))
        {
            _normalSurface = hit.normal;
            _tangentSurface = Vector3.Cross( transformBias.transform.right,hit.normal);
            _isGrounded =  true;

            _normalSurface = _normalSurface.normalized;
            _tangentSurface = _tangentSurface.normalized;

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
    
    
    
    
    
    
    
    
    
    
    
    // useful fonction
    public float ProjectAngle(Vector3 A, Vector3 B, Vector3 normal)
    {
        Vector3 a = Vector3.ProjectOnPlane(A, normal);
        Vector3 b = Vector3.ProjectOnPlane(B, normal);

        return Vector3.SignedAngle(a, b,Vector3.right);
    }
    
}
