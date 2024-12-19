using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CigPacket : MonoBehaviour
{

   

    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject slot4;
    public GameObject slot5;
    public GameObject slot6;
    public GameObject slot7;
public GameObject emptyItem;
     public GameObject itemSlot1;
    public GameObject itemSlot2;
    public GameObject itemSlot3;
    public GameObject itemSlot4;
    public GameObject itemSlot5;
    public GameObject itemSlot6;
    public GameObject itemSlot7;

    
    // Start is called before the first frame update
 
    void Start(){
        itemSlot1 = emptyItem;
        itemSlot2 = emptyItem;
        itemSlot3 = emptyItem;
        itemSlot4 = emptyItem;
        itemSlot5 = emptyItem;
        itemSlot6 = emptyItem;
        itemSlot7 = emptyItem;



    }

    public bool depositCig(GameObject Cig){
        bool deposited = false;
        if(itemSlot1 == emptyItem){
            //deposit one
            Cig.transform.position = slot1.transform.position;
            Cig.transform.rotation = slot1.transform.rotation;
            
            Cig.transform.parent = slot1.transform;
            itemSlot1 = Cig;
            deposited = true;
            
        }
        
        
        
        else if(itemSlot2 == emptyItem){
            Cig.transform.position = slot2.transform.position;
            Cig.transform.rotation = slot2.transform.rotation;
            Cig.transform.parent = slot2.transform;
            itemSlot2 = Cig;
             deposited = true;
            
        }else if(itemSlot3 == emptyItem){
            //deposit one
            Cig.transform.position = slot3.transform.position;
            Cig.transform.rotation = slot3.transform.rotation;
            Cig.transform.parent = slot3.transform;
            itemSlot3 = Cig;
             deposited = true;
            
        }else if(itemSlot4 == emptyItem){
            //deposit one
            Cig.transform.position = slot4.transform.position;
            Cig.transform.rotation = slot4.transform.rotation;
            Cig.transform.parent = slot4.transform;
            itemSlot4 = Cig;
             deposited = true;
            
        }else if(itemSlot5 == emptyItem){
            //deposit one
            Cig.transform.position = slot5.transform.position;
            Cig.transform.rotation = slot5.transform.rotation;
            Cig.transform.parent = slot5.transform;
            itemSlot5 = Cig;
             deposited = true;
            
        }else if(itemSlot6 == emptyItem){
            //deposit one
            Cig.transform.position = slot6.transform.position;
            Cig.transform.rotation = slot6.transform.rotation;
            Cig.transform.parent = slot6.transform;
            itemSlot6 = Cig;
             deposited = true;
            
        }else if(itemSlot7 == emptyItem){
            //deposit one
            Cig.transform.position = slot7.transform.position;
            Cig.transform.rotation = slot7.transform.rotation;
            Cig.transform.parent = slot7.transform;
            itemSlot7 = Cig;
             deposited = true;
        }

        if(deposited){
            Cig.layer = LayerMask.NameToLayer("NoHitRayCast");
        }

        return deposited;

        



    }




    public GameObject withdrawlCig(){
        GameObject itemToReturn = null;
        
        if(itemSlot7.GetComponent<Cig>()){
            itemToReturn = itemSlot7;
            itemSlot7 = emptyItem; 
        }else 
        if(itemSlot6.GetComponent<Cig>()){
            itemToReturn = itemSlot6;
            itemSlot6 = emptyItem;
        }else 
        if(itemSlot5.GetComponent<Cig>()){
            itemToReturn = itemSlot5;
            itemSlot5 = emptyItem;
        }else 
        if(itemSlot4.GetComponent<Cig>()){
            itemToReturn = itemSlot4;
            itemSlot4 = emptyItem;
        }else 
        if(itemSlot3.GetComponent<Cig>()){
            itemToReturn = itemSlot3;
            itemSlot3 = emptyItem;
        }else 
        if(itemSlot2.GetComponent<Cig>()){
            itemToReturn = itemSlot2;
            itemSlot2 = emptyItem;
        }else 
        if(itemSlot1.GetComponent<Cig>()){
            itemToReturn = itemSlot1;
            itemSlot1 = emptyItem;
        }
        
        if(itemToReturn != null){
            itemToReturn.layer = LayerMask.NameToLayer("Default");
        }
        return itemToReturn;


    }

}
