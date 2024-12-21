using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class goopSlow : MonoBehaviour
{
    public bool playerInGoop = false;
   public void OnTriggerEnter(Collider other){
    
        
         if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
       {
            playerInGoop = true;
            Debug.Log(other.gameObject);
           
            other.gameObject.GetComponent<PlayerMovementController>().goopSpeedModifier = .25f;
            
        }
   }

   public void OnTriggerExit(Collider other){
        
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && playerInGoop){
            playerInGoop = false;
            other.gameObject.GetComponent<PlayerMovementController>().goopSpeedModifier = 1f;
            
            
        }
   }
}
