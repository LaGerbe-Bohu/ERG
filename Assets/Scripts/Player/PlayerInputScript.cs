using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{

   public float getAxisVertical()
   {
      return Input.GetAxis("Vertical");
   }
   
   public float getAxisHorizontal()
   {
      return Input.GetAxis("Horizontal");
   }
   
   public bool getInputForward()
   {
      if ( this.getAxisVertical() > 0 )
      {
         return true;
      }

      return false;
   }
   
   public bool getInputBackward()
   {
      if ( this.getAxisVertical() < 0 )
      {
         return true;
      }

      return false;
   }
   
   public bool getInputRight()
   {
      if ( this.getAxisHorizontal() > 0 )
      {
         return true;
      }

      return false;
   }
   
   public bool getInputLeft()
   {
      if ( this.getAxisHorizontal() < 0 )
      {
         return true;
      }

      return false;
   }

   
   
}
