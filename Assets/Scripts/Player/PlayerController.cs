using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Script managing player movement
    /// This player controller work with no friction, because the script handle it itself. 
    /// </summary>

    [Header("Components")] 
    public Camera cameraPlayer;
    public Transform trPlayer;
    public Rigidbody rbPlayer;
    public CapsuleCollider cc;

    [Header("Values for movement")] 
    public AnimationCurve acceleration; // Curve for the acceleration and the break
    public AnimationCurve drag;

    [Header("Rotation")]
    public float dampingTime;

    [Header("Slopes movement")] 
    public AnimationCurve slopeCurvature;
    public LayerMask groundCheck;
    public float tresholdOfSlide; 
    
    [Header("Commands")] 
    public float deadZone = 0.2f;
    
    // private section

    private float verticalAxe;
    private float horizontalAxe;
    private float acc;

    private float time;
    private bool switchState = false;
    private float oldDampingTime;
    private float currentAngle;
    
    private Quaternion lookDr;
    private Vector3 lastPosition;
    private Vector3 oldDir;
    private Vector3 slopeDirection;
    
    
    void Start()
    {
        time = 0;
        acc = 0f;
        oldDampingTime = dampingTime;
        oldDir = trPlayer.transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        verticalAxe = Input.GetAxis("Vertical");
        horizontalAxe = Input.GetAxis("Horizontal");

        if (horizontalAxe <= deadZone && horizontalAxe >= -deadZone)
        {
            horizontalAxe = 0;
        }
        
        if (verticalAxe <= deadZone && verticalAxe >= -deadZone)
        {
            verticalAxe = 0;
        }
        
        dragEffect();

        
        dampingTime = oldDampingTime;
        

        
    }

    private bool isMoving()
    {
        return horizontalAxe > 0 || verticalAxe > 0 || horizontalAxe < 0 || verticalAxe < 0;
    }

    
    private void dragEffect()
    {
        
        if (isMoving())
        {
   
            if (!switchState)
            {
                time = 0;
                rbPlayer.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |
                                       RigidbodyConstraints.FreezeRotationY;
                switchState = true;
            }
            
            time += Time.deltaTime;
            var lastTime = acceleration.keys[^1].time;
            time = Mathf.Clamp(time, 0, lastTime);
          
            acc = acceleration.Evaluate(time);
        }
        else 
        {
     
            if (switchState)
            {
                time = 0;
                switchState = false;
            }
            
            if(time >= drag.keys[^1].time && currentAngle <= tresholdOfSlide)
            {
                rbPlayer.constraints = RigidbodyConstraints.FreezeAll;
            }
            
            time += Time.deltaTime;
            var lastTime = drag.keys[^1].time;
            time = Mathf.Clamp(time, 0, lastTime);
            
            acc = drag.Evaluate(time);
        }

    }
    public void MovePlayer()
    {
        /*
         Show velocity speed 
         
        Vector2 velo = new Vector2(rbPlayer.velocity.x, rbPlayer.velocity.z);
        Debug.Log(velo.magnitude);
       */ 
        
        var transform1 = cameraPlayer.transform;
        var right = transform1.right ;
        var dirR = new Vector3(right.x, 0, right.z) * horizontalAxe;
        
        var forward = transform1.forward;
        var dirF = new Vector3(forward.x, 0, forward.z) * verticalAxe;
        var dir = (dirR + dirF).normalized;

        if (dir == Vector3.zero)
        {
            dir = oldDir;
        
        }

        oldDir = dir;
        lookDr = Quaternion.LookRotation(dir);

        var d = new Vector3(dir.x, 0, dir.z);
        
       float y1 = slopeDirection.y >= 0 ? 0 : 1;
       var slopeCompensation = slopeCurvature.Evaluate(currentAngle);
       
       Debug.Log(slopeCompensation +" Angle "+ currentAngle);
       
        rbPlayer.velocity = new Vector3(d.x * acc *slopeCompensation ,rbPlayer.velocity.y + slopeDirection.y * y1 ,d.z *acc * slopeCompensation);

    }

    
    private void FixedUpdate()
    {
       
        MovePlayer();
        currentAngle = 0;
      

        if (Physics.SphereCast(trPlayer.position,cc.radius,-trPlayer.transform.up, out var hit,3f,groundCheck))
        {
            slopeDirection = Vector3.Cross(trPlayer.right,hit.normal );

            currentAngle = Vector3.SignedAngle(slopeDirection,trPlayer.forward,trPlayer.right );
            
            //Debug.Log(currentAngle);
            
            
            Debug.DrawRay(trPlayer.position+trPlayer.forward*cc.radius, slopeDirection * 150f, Color.yellow);
            Debug.DrawRay(trPlayer.position+trPlayer.forward*cc.radius, trPlayer.forward * 150f, Color.red);
        
        }
         
        trPlayer.localRotation = Quaternion.Slerp(trPlayer.transform.localRotation,lookDr, Time.deltaTime *dampingTime);
    }
}
