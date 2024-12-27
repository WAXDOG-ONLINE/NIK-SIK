using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject respawnPoint;
    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
       {
          other.gameObject.GetComponent<CharacterController>().enabled = false;
         other.gameObject.transform.position = respawnPoint.transform.position;
        other.gameObject.GetComponent<ActionManager>().takeDamage(1);
            other.gameObject.GetComponent<CharacterController>().enabled = true;
         
       }

       //Teleport player
      
    }
}
