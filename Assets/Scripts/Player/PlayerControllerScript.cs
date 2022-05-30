using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{

    /// <summary>
    /// Script de déplacement du joueur
    /// </summary>

    [Header("Conpenents")] 
    public Transform target;
    public PlayerInputScript PIS;
    public Rigidbody RB;

    [Header("valeurs")] 
    public float acceleration;
    public float maxVitesse;
    public float torqueMaxVitesse;
    
    private float axisForward;
    private float axisRight;

    private Vector3 acc;
    
    private Vector3 torque;
    private float torqueAcceleration;
    private float t;

    [HideInInspector]
    public Vector3[] lstofPoint;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        axisForward = PIS.getAxisVertical();
        axisRight = PIS.getAxisHorizontal();
        
        
    
    }
    

    private void FixedUpdate()
    {
        acc = (target.forward * axisForward * acceleration);
        torqueAcceleration = ( axisRight * torqueMaxVitesse);
        
            
        target.transform.RotateAround(target.transform.position,Vector3.up,torqueAcceleration );
        
        Vector3 clampedVelo = Vector3.ClampMagnitude(RB.velocity,maxVitesse);
        RB.velocity = new Vector3(clampedVelo.x, RB.velocity.y, clampedVelo.z); 
        RB.AddForce(acc,ForceMode.Acceleration);
        
    }
}
