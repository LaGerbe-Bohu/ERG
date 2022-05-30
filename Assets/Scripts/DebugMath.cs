using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMath : MonoBehaviour
{
    public  float _HeightGrasse = 1;
    public float _WidthGrasse = 1;
    public int _NomberOfDetail = 1;
    
    // Start is called before the first frame update
    void Start()
    {


   
       for(float i = 0; i <=_HeightGrasse; i+= _HeightGrasse)
        {
            for(int j = 0; j <=_WidthGrasse; j+=1)
            {
                Debug.Log(i.ToString()+ " " + j.ToString());     
            }
        }
    
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
