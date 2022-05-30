using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Script managing player movement
    /// </summary>

    [Header("Components")] 
    public Camera cameraPlayer;
    public Transform trPlayer;
    public Rigidbody rbPlayer;


    [Header("Values for movement")] 
    public AnimationCurve acceleration; // Curve for the acceleration and the break
    public AnimationCurve drag;

    [Header("Rotation")]
    public float dampingTime;

    [Header("Commandes")] 
    public float deadZone = 0.2f;
    
    // private section

    private float verticalAxe;
    private float horizontalAxe;
    private float acc;

    private Quaternion lookDr;
    private Vector3 lastPosition;
    private float speed;
    private float time;
    private bool startAcc = false;
    private bool startBrk = false;
    private float oldDampingTime;
    private bool pressed = false;
    private Vector3 oldDir;
    void Start()
    {
        speed = 0;
        time = 0;
        acc = 0f;
        oldDampingTime = dampingTime;
    }
    // Update is called once per frame
    void Update()
    {
        verticalAxe = Input.GetAxisRaw("Vertical");
        horizontalAxe = Input.GetAxisRaw("Horizontal");

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

    
    private void dragEffect()
    {

        var velocity = rbPlayer.velocity;
        Vector2 velo = Vector2.ClampMagnitude(new Vector2(velocity.x,velocity.z),acc);
        rbPlayer.velocity = new Vector3(velo.x, rbPlayer.velocity.y, velo.y);
        speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        
    
        if (horizontalAxe > 0 || verticalAxe > 0 || horizontalAxe < 0 || verticalAxe < 0 )
        {
   
            if (!startAcc)
            {
                time = 0;
                startBrk = false;
                startAcc = true;
            }
            
            time += Time.deltaTime;
            var lastTime = acceleration.keys[^1].time;
            time = Mathf.Clamp(time, 0, lastTime);
          
            acc = acceleration.Evaluate(time);
        }
        else if(speed > 0f)
        {
     
            if (!startBrk)
            {
                time = 0;
                startBrk = true;
                startAcc = false;
            }
            
            time += Time.deltaTime;
            var lastTime = drag.keys[^1].time;
            time = Mathf.Clamp(time, 0, lastTime);
            
            acc = drag.Evaluate(time);
        }

    }
    public void MovePlayer()
    {

      
        
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
        rbPlayer.velocity = new Vector3(d.x * acc,rbPlayer.velocity.y,d.z *acc);

    }

    
    private void FixedUpdate()
    {
  

        
        MovePlayer();
     
        
        
        
        trPlayer.localRotation = Quaternion.Slerp(trPlayer.transform.localRotation,lookDr, Time.deltaTime *dampingTime);
        
  



    }
}
