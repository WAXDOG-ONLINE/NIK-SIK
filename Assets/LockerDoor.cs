using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class LockerDoor : MonoBehaviour
{
    public GameObject doorP;
    public float doorAngle;

    public float timer;
    public float startAngle = 0;

    public bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {

    
        if(open){
            
            if(timer < .7f ){
                timer = timer + Time.deltaTime;
                doorAngle =  Mathf.Lerp(0,-166.65f,timer/.7f);
                
            }
         
    }else{
        
           if(timer < .7f ){
                timer = timer + Time.deltaTime;
                  doorAngle =  Mathf.Lerp(-166.65f,0,timer/.7f);
            }
    }
        doorP.transform.localEulerAngles = new Vector3(0,doorAngle,0);
    }
public void openClose(){

open = !open;
timer = 0;
startAngle = doorP.transform.localEulerAngles.y;

                    


}
   
}
