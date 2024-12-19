using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.AI;
[RequireComponent(typeof(AudioSource))]
public class ItemInfo : MonoBehaviour
{
    public AudioSource dropSound;
    public GlobalVariables globalVariables;
    public float sellValue = 1;
    public bool colliding = false;

    [Header("MUST BE SET MANUALLY IN ITEM PREFAB")]
    public Material ghostMaterial;

   
//used for disabling nav scripts when picking up entities with nav agents


    public bool isHuntable = false;

    public Vector3 parentPositionOffset;
    public Vector3 parentRotationOffset;

    public Vector3 inventoryRotationOffset;

    private bool beingMoved = false;

    

   
   
   List<GameObject> myChildrenList = new List<GameObject>();

   List<Material> myChildrenMatsList = new List<Material>();

   GameObject editingObject;

    //meshSkin renderer????

    
    //unused
    public bool isOnGround = true;
    

    // Start is called before the first frame update
    void Start()
    {
        globalVariables = GameObject.Find("GlobalVariables").GetComponent<GlobalVariables>();
        dropSound = GetComponent<AudioSource>();
        dropSound.playOnAwake = false;
        dropSound.loop = false;
        dropSound.maxDistance = 10;
        dropSound.volume = 0.5f;
        dropSound.dopplerLevel = 5;
        dropSound.clip = globalVariables.dropSoundClip;
        //add rigid body at start to all items
        
        transform.AddComponent<Rigidbody>();
        transform.GetComponent<BoxCollider>().isTrigger = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        

        //SET GHOST MATERIAL AUTOMAGICLY

        
        //loop for gameobjects, count the amount with meshes or skinnedmeshes, add them to an list, add their materials to an list
 
        
 
        editingObject = transform.gameObject;
        
        //if the children have children they should be added to the list. unless they are armetures?
        
        addMeshedChildrenToList(editingObject);
       
      
        


       
    }
    
    
    public void addMeshedChildrenToList(GameObject editingObject){

        

    
        foreach(Transform child in editingObject.transform){
           
            if(child.GetComponent<MeshRenderer>() | child.GetComponent<SkinnedMeshRenderer>()){
               myChildrenList.Add(child.gameObject);
                
                if(child.GetComponent<MeshRenderer>()){
                    myChildrenMatsList.Add(child.GetComponent<MeshRenderer>().material);
                }
                 if(child.GetComponent<SkinnedMeshRenderer>()){
                   myChildrenMatsList.Add(child.GetComponent<SkinnedMeshRenderer>().material);
                }
            }
            if(child.childCount > 0){
                addMeshedChildrenToList(child.gameObject);
            }




        }

        



    }
    public void setGhostMaterial(){
        colliding = false;
        beingMoved = true;
        

        for(int i = 0; i < myChildrenList.Count; i++){
            if(myChildrenList[i].GetComponent<MeshRenderer>()){
                myChildrenList[i].GetComponent<MeshRenderer>().material = ghostMaterial;
                myChildrenList[i].GetComponent<MeshRenderer>().material.color =  new Color(0,1,0,0.4f);
            }else{
                myChildrenList[i].GetComponent<SkinnedMeshRenderer>().material = ghostMaterial;
                myChildrenList[i].GetComponent<SkinnedMeshRenderer>().material.color =  new Color(0,1,0,0.4f);
            }
            
            
        }

       



   
        //set meshes postion to inventory cursor, check collision with other items, if colliding set color to red dissalow placement, if good, set green allow
     
       
    }

    public void restoreMaterial(){
        beingMoved = false;
        for(int i = 0; i < myChildrenList.Count; i++ ){
            if(myChildrenList[i].GetComponent<MeshRenderer>()){
            myChildrenList[i].GetComponent<MeshRenderer>().material = myChildrenMatsList[i];
            }else{

                myChildrenList[i].GetComponent<SkinnedMeshRenderer>().material = myChildrenMatsList[i];

            }

        }
     
    }

    private void  OnTriggerStay(Collider other){


        //check if item is on floor


       //check if this item is moving, if other collider is an item, and if other collider is Raycast hitable(set to false when part of another item)
       if(beingMoved && other.gameObject.GetComponent<ItemInfo>() &&  other.gameObject.layer != LayerMask.NameToLayer("NoHitRayCast")){
        
        for(int i = 0; i < myChildrenList.Count; i++ ){
        if(myChildrenList[i].GetComponent<MeshRenderer>()){
        myChildrenList[i].GetComponent<MeshRenderer>().material.color = new Color(1,0,0,0.4f);

        }else{

            myChildrenList[i].GetComponent<SkinnedMeshRenderer>().material.color = new Color(1,0,0,0.4f);

        }
                                            }
   




        
        colliding = true;
       }

    }


private void OnCollisionEnter(Collision other){
    if(!transform.GetComponent<NavMeshAgent>()){
    
    if(other.gameObject.layer == LayerMask.NameToLayer("whatIsGround")){
        dropSound.pitch = UnityEngine.Random.Range(0.8f,1.5f);
    dropSound.Play();
    }}



}
   


    
}
