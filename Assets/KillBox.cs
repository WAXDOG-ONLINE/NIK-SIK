using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
       {
           other.gameObject.GetComponent<ActionManager>().health += -110;
       }
    }
}
