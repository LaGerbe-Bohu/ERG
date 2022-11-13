using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(Rigidbody)))]
public class CharacterController : MonoBehaviour
{
    //
    /// Faut que tu fasse en sorte que le joueur spawn en regardant devant lui
    ///
    
    
    // ref for the fps view : https://gist.github.com/KarlRamstedt/407d50725c7b6abeaf43aee802fdd88e
    
    [Header("Compenents")] 
    public Camera cameraPlayer;
    public Transform playerBody;

    [Header("Attributes")] 
    public float mouseSensitivy = 35f;
    public float maxRotation = 90f;
    
    
    // Some private values

    private Quaternion baseQuaternion; // this quaternion is for store the first rotation of cam
    private Vector2 rotation;
    
    
    const string xAxis = "Mouse X"; // axis const
    const string yAxis = "Mouse Y";
    
    void Start()
    {
        // lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        
        rotation = new Vector2(0f,0f);
    }       
    
    void Update()
    {
        rotation.x += Input.GetAxis(xAxis) *  mouseSensitivy * Time.deltaTime;
        rotation.y += Input.GetAxis(yAxis) *  -mouseSensitivy * Time.deltaTime;
        rotation.y =  Mathf.Clamp(rotation.y, -maxRotation, maxRotation);
        

        
        Quaternion xQuat = Quaternion.Euler(0.0f,rotation.x, 0.0f);
        Quaternion yQuat = Quaternion.Euler(rotation.y,0.0f, 0.0f);

        cameraPlayer.transform.localRotation = xQuat*yQuat;

    }
}
