using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OnDebugShader : MonoBehaviour
{

    public Material Mat;
    public Transform CenterEllipse;
    public Transform RightFoot;
    public Transform LeftFoot;
    public LayerMask GroundLayer;
    
    
    public Color colorBalanced;
    public Color notBalanced;

    public ProceduralAnimationBalanced PA;
    
    public float Angle;

    // Update is called once per frame
    void Update()
    {
        
        if (PA.isInEllipseOfMass(CenterEllipse.transform.position))
        {
            Mat.SetColor("_ColorOfEllipse",colorBalanced);
        }
        else
        {
            Mat.SetColor("_ColorOfEllipse",notBalanced);
        }

        Mat.SetVector("_centerEllipse", PA.getCenterEllipse());
        Mat.SetVector("_centerOfMass", PA.target.position);
        Mat.SetFloat("_semiMinorAxis",PA.getDistanceBetweenLegs());
        Mat.SetFloat("_semiMajorAxis",PA.maxDistanceStep);
        Mat.SetFloat("_angle",  PA.getAngleOfEllipse());
        
        Ray r = new Ray(this.transform.position + new Vector3(0,1,0),Vector3.down*4);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, GroundLayer))
        {
            this.transform.rotation = Quaternion.LookRotation(-hit.normal);
            this.transform.position = new Vector3(this.transform.position.x,hit.point.y + hit.normal.y/80f,this.transform.position.z);
        }
        
    }
}
