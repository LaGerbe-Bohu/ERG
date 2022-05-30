using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    /// <summary>
    /// Procedural animation
    /// </summary>


    [Header("Value")] 
    public float distanceStep;

    [Header("Conpenents")] 
    public Transform targetTransform;
    public Transform rightFoot;
    public Transform leftFoot;



    private Vector3 oldPositionTarget;
    private Vector3 centerOfCircle;
    public Vector3 currentRightFoot;
    
    private void OnDrawGizmos()
    {
        centerOfCircle = rightFoot.position + (leftFoot.position - rightFoot.position) / 2;
        centerOfCircle = Vector3.ProjectOnPlane(centerOfCircle, Vector3.up);

        Gizmos.color = Color.red;
        
        
        Gizmos.DrawSphere(centerOfCircle,0.1f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentRightFoot,0.1f);
        
        
        
    }


    private bool isInCircle(Vector3 position)
    {
        float distance = Vector3.Distance(centerOfCircle,position);

        return distance <= distanceStep;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        oldPositionTarget = targetTransform.position;
        currentRightFoot = rightFoot.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        rightFoot.transform.position = currentRightFoot;
        
        if (!isInCircle(targetTransform.position))
        {
            Vector3 direction = (targetTransform.position - oldPositionTarget).normalized;
            oldPositionTarget = targetTransform.position;
            
            
            currentRightFoot = targetTransform.position + (direction * distanceStep) - targetTransform.right * 0.1f;
        }
        
    }
    
    
    
}
