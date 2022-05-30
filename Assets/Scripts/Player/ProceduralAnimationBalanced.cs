using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimationBalanced : MonoBehaviour
{
    /// <summary>
    /// proceduralAnimation
    /// </summary>

    [Header("Conpenents")] 
    public Transform target;

    public PlayerControllerScript PCS;
    public Rigidbody RB;
    public Transform RightFoot;
    public Transform LeftFoot;

    [Header("Values")] 
    public float maxDistanceStep;
    public float SpaceBetweenfoot;
    public float distanceCheckGround = 100;
    public LayerMask LayerGround;
    public Transform Ground;
    
    
    private Vector3 targetRight;
    private Vector3 targetLeft;
    
    private Vector3 currentRight;
    private Vector3 currentLeft;

    private Vector3 newPositionRight;


    private Vector3 posRight;
    private Vector3 posLeft;

    private Vector3 posRightDirection;
    private Vector3 posLeftDirection;
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color =Color.black;
        
        Gizmos.DrawRay(LeftFoot.transform.position,LeftFoot.forward*100f );
        
        Gizmos.DrawRay(RightFoot.transform.position,RightFoot.forward*100f );
  
      
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(posRight,0.05f);
        Gizmos.DrawSphere(posLeft,0.05f);
        
        
        Gizmos.DrawRay(currentRight,posRightDirection * 100f);
        
        */
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Ray r = new Ray(target.transform.position + new Vector3(0,1,0) + target.right * -SpaceBetweenfoot  ,Vector3.down * distanceCheckGround );
        currentRight = findGround(r);
        
        r = new Ray(target.transform.position + new Vector3(0,1,0) + target.right * SpaceBetweenfoot ,Vector3.down * distanceCheckGround );
        currentLeft = findGround(r);

        posRightDirection = target.transform.forward;
        posLeftDirection = target.transform.forward;
        newPositionRight = currentRight;
    }

    public Vector3 findGround(Ray r)
    {
        RaycastHit hit;
        Physics.Raycast(r, out hit, distanceCheckGround, LayerGround);

        return hit.point;
    }
    
    
    public void NextStep()
    {
        Vector3 posRight = Vector3.ProjectOnPlane(RightFoot.transform.position, Vector3.up);
        Vector3 posLeft = Vector3.ProjectOnPlane(LeftFoot.transform.position, Vector3.up);
        Vector3 posMass = Vector3.ProjectOnPlane(target.transform.position, Vector3.up);

        
        Debug.Log((Vector3.Distance(posMass, posRight) - Vector3.Distance(posMass, posLeft) ) );
        
        if ( ( Mathf.Abs( Vector3.Distance(posMass, posRight) - Vector3.Distance(posMass, posLeft) ) < 0.1f) || (Vector3.Distance(posMass, posRight) > Vector3.Distance(posMass, posLeft)) )
        {
            Vector3 vectorLegs = (posMass - posLeft );
        
            Vector3 center = posMass + vectorLegs;
            
            Ray r = new Ray(center + new Vector3(0, this.transform.position.y + 5, 0), Vector3.down * 500f);
            currentRight = findGround(r);
            

        }
        else
        {
            Vector3 vectorLegs = (posMass - posRight );
            Vector3 center = posMass + vectorLegs;
            
            Ray r = new Ray(center + new Vector3(0, this.transform.position.y + 5, 0), Vector3.down * 500f);
            currentLeft = findGround(r);
            
        }
 

    }
    
    
    // Update is called once per frame
    private void Update()
    {

        RightFoot.transform.position = currentRight;
        LeftFoot.transform.position = currentLeft;

        
       
        posRight = new Vector3(target.position.x,Ground.transform.position.y,target.position.z) - target.right * 0.1f;
        posLeft = new Vector3(target.position.x,Ground.transform.position.y,target.position.z) + target.right * 0.1f;

        if ( Mathf.Abs(Vector3.Angle(posRightDirection,RightFoot.forward)) > 30f)
        {
            currentRight = posRight;
             posRightDirection = RightFoot.forward;
        }
        
        if ( Mathf.Abs(Vector3.Angle(posLeftDirection,RightFoot.forward)) > 30f)
        {
             currentLeft = posLeft;
             posLeftDirection = LeftFoot.forward;
        }

        
        

        if (!isInEllipseOfMass(target.transform.position))
        {
            
            NextStep();
            
        }

    }
    

    public float getAngleOfEllipse()
    {
        float d =   -Vector3.SignedAngle(target.right, (LeftFoot.position - RightFoot.position).normalized,Vector3.up);

        return d = Mathf.Abs(d - 90);
    }

    public Vector3 getCenterEllipse()
    {
        Vector3 getCenter = RightFoot.position + (LeftFoot.position - RightFoot.position) / 2;
        getCenter = Vector3.ProjectOnPlane(getCenter, Vector3.up);
        return getCenter;
    }

    public float getDistanceBetweenLegs()
    {
        Vector3 p1 = Vector3.ProjectOnPlane(RightFoot.position, Vector3.up);
        Vector3 p2 = Vector3.ProjectOnPlane(LeftFoot.position, Vector3.up);

        
        return  Vector3.Distance(p1, p2);
    }
    
    public bool isInEllipseOfMass(Vector3 pos)
    {

        Vector3 getCenter = getCenterEllipse();
        pos = Vector3.ProjectOnPlane(pos, Vector3.up);
        
        Vector2 origin = new Vector2(getCenter.x - pos.x,getCenter.z -  pos.z);
        
        float d = getAngleOfEllipse() * (Mathf.PI/180f);
        float dist = getDistanceBetweenLegs();

        return (Mathf.Pow(maxDistanceStep,2)*Mathf.Pow(Mathf.Sin(d),2) + Mathf.Pow(dist,2)*Mathf.Pow(Mathf.Cos(d),2))* Mathf.Pow(origin.x,2) + 2*(Mathf.Pow(dist,2)-Mathf.Pow(maxDistanceStep,2))
            *Mathf.Sin(d)*Mathf.Cos(d)*origin.x*origin.y + (Mathf.Pow(maxDistanceStep,2) * Mathf.Pow(Mathf.Cos(d),2) + Mathf.Pow(dist,2)*Mathf.Pow(Mathf.Sin(d),2) )*Mathf.Pow(origin.y,2) <= Mathf.Pow(maxDistanceStep,2)*Mathf.Pow(dist,2);
    }



}
