using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
[ExecuteInEditMode]
public class SellBin : MonoBehaviour
{
Collider[] hitColliders;
public TextMeshPro valueText;
private Vector3 center;
public float radius = 1.2f;
public Vector3 size = new Vector3(0,0,0);
public UnityEngine.Color gizmoColor;

public bool displayGizmo = true;
 public float totalValue = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        center = transform.position;
        hitColliders = Physics.OverlapBox(center, size);
        
    }

    // Update is called once per frame
    void Update()
    {
        totalValue = 0;
        hitColliders = Physics.OverlapBox(center, size);

        for(int i = 0; i< hitColliders.Length; i++){
            
            if(hitColliders[i].transform.GetComponent<ItemInfo>()){

            totalValue = totalValue + Mathf.Ceil(hitColliders[i].transform.GetComponent<ItemInfo>().sellValue);
            

            }
            


        }


        
        if(totalValue < 10){
            valueText.text = "00" + totalValue + "$";
        }
        if(totalValue > 9 && totalValue < 99 ){
            valueText.text = "0" + totalValue + "$";
        }
        if(totalValue > 99){
            valueText.text =  totalValue + "$";
        }
        
    }



public void SellItems(){
   
            


        
    
    for(int i = 0; i < hitColliders.Length; i++){
        if(hitColliders[i].GetComponent<ItemInfo>()){
       if(hitColliders[i].GetComponent<ItemInfo>().sellValue >0){
        Destroy(hitColliders[i].gameObject);
       }
        }
    }

    hitColliders = new Collider[0];


}
    void OnDrawGizmos(){
        if(displayGizmo){
            Gizmos.color = gizmoColor;
        
         Gizmos.DrawCube(center,  size *2);
        }


    }


}
