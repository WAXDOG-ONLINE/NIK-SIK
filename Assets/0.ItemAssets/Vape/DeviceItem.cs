using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DeviceItem : MonoBehaviour
{

    public float batteryPercentage;
    public float batteryDrainRate;

    public string compatibility;
    public float batteyChargeRate = 1;
    public PodItem attachedPod;

    public Light vLight;

    public Color lightColorFull = Color.green;
    public Color lightColorEmpty = Color.red;

    public float maxLightValue = 20f;

    public bool attachedCharger;

    // Start is called before the first frame update
    void Start()
    {
        
      
    }

    // Update is called once per frame
    void Update()
    {
        if(vLight.intensity > 0){
        vLight.intensity = vLight.intensity - 0.25f; 
        }
        if(vLight.intensity <= 0){
            vLight.enabled = false;
        }

        if(attachedCharger && batteryPercentage <= 100){
            batteryPercentage = batteryPercentage + Time.deltaTime*batteyChargeRate;
        }
        
    }

    
    public void BatteryDrain(){
        if (batteryPercentage  > 0){
        batteryPercentage = batteryPercentage - Time.deltaTime*batteryDrainRate;
        }
    }
     

    public void updateLight(){
        if(vLight.intensity < maxLightValue && batteryPercentage > 0f){
                                vLight.intensity = vLight.intensity + 0.5f;  
                                vLight.enabled = true;
                                                        }
    vLight.color = Color.Lerp(lightColorEmpty,lightColorFull,batteryPercentage/100);
    


     }






     public bool checkCompatiblity(PodItem toBeAttached){
        
        if( compatibility == toBeAttached.compatibility){
            return true;
        }else{
            return false;
        }



    }
    public void attachPod(PodItem toBeAttached){

        toBeAttached.transform.parent = transform;
        toBeAttached.transform.position = transform.position;
        toBeAttached.transform.rotation = transform.rotation;
        toBeAttached.transform.localPosition = toBeAttached.podPositionOffset;
        toBeAttached.gameObject.layer = LayerMask.NameToLayer("NoHitRayCast");

        attachedPod = toBeAttached;

    }


    public void dettachPod(){
       attachedPod.gameObject.layer = LayerMask.NameToLayer("Default");
       
        attachedPod = null;
    

    }

   
}
