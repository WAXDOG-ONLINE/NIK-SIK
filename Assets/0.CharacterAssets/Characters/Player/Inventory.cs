using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Inventory : MonoBehaviour
{

private quaternion lockedRot = new quaternion(0,0,0,0);
    private bool inventoryRetrieved; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void retrieveInventroy(){


        if(inventoryRetrieved == true){
         transform.position = new Vector3(transform.position.x, transform.position.y + 200, transform.position.z);
         lockedRot = transform.rotation;
         inventoryRetrieved = false;
        }

        transform.rotation = lockedRot;
        

    }

    public void unRetrieveInventory(){
        if (inventoryRetrieved == false){
            transform.rotation = new quaternion(0,0,0,0);
            transform.position = new Vector3(transform.position.x, transform.position.y - 200, transform.position.z);
            inventoryRetrieved = true;
        }
    }
}
