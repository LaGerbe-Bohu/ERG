using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAFoot : MonoBehaviour
{

    [Header("Conpenent")] 
    public Transform target;

    [Header("Values")] 
    public float distanceCheckGround;
    public LayerMask LayerGround;
    public float footSpace;
    public float offestStep;
    public Vector3 offset;
    public float maxDistanceStep;
    public AnimationCurve StepMouvement;
    public bool inverse = false;
    
    private Vector3 newPosition;
    private Vector3 currentPosition;
    private float distance;


    private void OnDrawGizmos()
    {
     
        Gizmos.color = Color.black;

        
        Gizmos.DrawRay( target.position + new Vector3(0,1,0) + (target.position - this.transform.position).normalized * maxDistanceStep ,Vector3.down * distanceCheckGround);
        
        Gizmos.DrawSphere(newPosition,0.05f);
    }

    // Start is called before the first frame update
    void Start()
    {
        newPosition = this.transform.position;
        currentPosition = newPosition;
        distance = 0;
    }

    private void Update()
    {
        transform.position = newPosition;
        
        Ray r = new Ray(target.position + new Vector3(0,1,0) + (target.position - this.transform.position).normalized * maxDistanceStep ,Vector3.down * distanceCheckGround );
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, distanceCheckGround, LayerGround))
        {
            if (Vector3.Distance(hit.point, newPosition) >= maxDistanceStep)
            {
                newPosition = hit.point;
            }
        }
        
       
        
       
        
        float val = 0;
        if (distance != 0)
        { 
            val = distance;
        }
        else
        {
            val = 0;
        }
      
        


        currentPosition = Vector3.Lerp(currentPosition, newPosition,val);
        




    }

    // Update is called once per frame
    void FixedUpdate()
    {

   





    }
    
    
}
