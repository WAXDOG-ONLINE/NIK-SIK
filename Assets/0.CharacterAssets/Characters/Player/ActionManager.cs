
using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering.Fullscreen.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.iOS;
using UnityEngine.UI;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class ActionManager : MonoBehaviour
{

    public PlayerMovementController playerMovementController;
    public FullScreenEffectController fullScreenEffectController;
    private bool swayDirection = true;
    private float swayR = 0f;
    
    
    public LayerMask RayCastHitable;

    

    [SerializeField]
    private GameObject cameraParent;
    
    
    
    [Header("AudioSources")]
        public AudioSource vapeSound;
        public AudioSource pukeSound;
    
    
    
    [Header("Animation")]

        [SerializeField]
        private ParticleSystem smoke;

        [SerializeField]
        private ParticleSystem puke;
        [SerializeField]
        private Animator animator;
        public AudioEchoFilter delayEffect;
        public AudioReverbFilter reverbFilter;
        
    
    
    
    [Header("ItemManagement")]
        public GameObject leftHand;
        
        public GameObject rightHand;

        public ItemInfo itemLeftHand;

        public ItemInfo itemRightHand;

        private GameObject activeHand1;

        private GameObject activeHand2;
        [HideInInspector]
        public ItemInfo activeItem1;
        [HideInInspector]
        public ItemInfo activeItem2;


        [SerializeField]
        private Inventory inventory;

        private bool inventoryOpen;
        private bool attemptingInventoryPlacementLeft = false;

        private bool attemptingInventoryPlacementRight= false;
        [SerializeField]    
        private Light inventoryLight;


    [Header("ActionManagement")]
    
        private bool isPerformingAction = false;


        public bool isVaping = false;
    

        private float smokeTimer = 0;
   
    [Header("PlayerStats")]
        public float health = 100;
        public float sickness = 0;
        public float craving = 0;
        [SerializeField]
        private float pickUpRange = 100f;

        [SerializeField]
        private float sicknessMultiplier = 1f;

        [SerializeField]
        private ItemInfo emptyItem;

        private bool isFillingPod = false;
        public TextMeshProUGUI statusBars;

        public float cashBalace = 0;

        private bool isDrinking = false;
        [HideInInspector]
        public bool isPerformingLongAction = false;
        private float pukeTimer = 15;

        [Header("UISTATS")]
        private float currentDeviceBatteryPercentage = 100;
        private float currentPodJuicePercentage = 100;

        private float speedModifier = 1f;
 
    void Update()
    {
        if(health< 0 ){
            Application.Quit();
            Time.timeScale = 0;


        }
 //START TEMP UI BLOCK
            
 statusBars.text = 
 "Juice" + (int) currentPodJuicePercentage
 +  "<br>Battery" + (int) currentDeviceBatteryPercentage
 +  "<br>Sickness" + (int) (sickness)
 +  "<br>craving" + (int) (craving)
 + "<br>Health " + (int) (health);
 //END TEMP UI BLOCK      


//inventory
 if(Input.GetButton("Inventory")){
    inventory.retrieveInventroy();
    inventoryOpen = true;
    inventoryLight.enabled = true;

   

 }else{
    inventory.unRetrieveInventory();
    inventoryOpen = false;
    inventoryLight.enabled = false;
 }

 tryPlaceInventoryLeft();
  tryPlaceInventoryRight();
  SicknessManager();
 //tryPlaceInventoryRight();

//MY STUFFF!!!

//craving 
if(isVaping){
    if(craving > 0){
    craving = craving - 7*Time.deltaTime;
    }
}else{
    if(craving < 100){
    craving = craving + 1*Time.deltaTime;
    }
}

fullScreenEffectController.setAnxietyIntensity(craving);


       //Inspect
if (Input.GetButton("Inspect") ){

        animator.SetBool("isInspecting", true);
     }else{
            animator.SetBool("isInspecting", false);
     }   
       
       
       
        
    
//if click with empty hand, and vape in left remove

if(Input.GetButton("DropLeft")){

tryDropItem(true);



}


if(Input.GetButton("DropRight")){

tryDropItem(false);



}



//if fire 1 and 2 check for possible combo actions
if(Input.GetButton("Fire1")&& Input.GetButton("Fire2") && isPerformingAction == false){
   tryDettachPod(false);  
   tryDettachPod(true);  
   tryAttachPod(false);
   tryAttachPod(true);
   tryFillPod(true);
   tryFillPod(false);
   tryDepositCig(true);
   tryDepositCig(false);
   tryWithdrawlCig(true);
   tryWithdrawlCig(false);
   tryInteract();
}else{
    
}


//use Item Left if mouse1
if(Input.GetButton("Fire1") || Input.GetButton("Fire2")){
    bool chirality = false;
   if(Input.GetButton("Fire1")){
    chirality = true;
   }
    if(Input.GetButton("Fire2")){
    chirality = false;
   }


  
    tryPickUp(chirality);
    tryPlaceCharger(chirality);
    tryPlaceKey(chirality);
    tryVape(chirality);
    tryDepositCash(chirality);
    tryDrinkBeer(chirality);
 
    trySmokeCig(chirality);
    //for functions that do not require items or chirality

    tryInteract();
   




}else{

     //animation for not vaping
                animator.SetBool("isChuffing?", false);      
                
}






//reset able to perform action when no action buttons are being held
if(Input.GetButton("Fire1") != true && Input.GetButton("Fire2") != true && isPerformingLongAction == false){
    isPerformingAction = false;
    isFillingPod = false;
    //stops vaping if you were vaping // maybe can move into vaping funfction, please check inventory handeling
    if(isVaping == true){
        
        isVaping = false;
        vapeSound.Stop();
        smoke.Play();
        smokeTimer = 2;//scale with time vaping
    }
    if(isDrinking ==true){
        isDrinking = false;
    }
    
    
}

if(smokeTimer >= 0){
    smokeTimer = smokeTimer - 1*Time.deltaTime;
    }
if(smokeTimer <= 0){
        smoke.Stop();

    }





       
    }

//action functions
#region interact functions
public void tryInteract(){



if(isPerformingAction == false){
var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
            
        var selection = hit.transform.gameObject;
            //check if hit object is item
            

        tryActivateButton(selection);
        tryOpenLocker(selection);
        trySellItems(selection);
        tryBuyVendingMachine(selection);
        tryPurchase(selection);
        tryPlaySlotMachine(selection);
            

            }
}}

public void tryActivateButton(GameObject selection){
    if(selection.GetComponent<GameButton>()){
                isPerformingAction = true;
                selection.GetComponent<GameButton>().activate();
                
                }
}
private void tryOpenLocker(GameObject selection){


            //check if hit object is item
            
            if(selection.GetComponent<LockerDoor>()){
                isPerformingAction = true;
                selection.GetComponent<LockerDoor>().openClose();
                
                
                }

  



}
private void trySellItems(GameObject selection){

    
                //check if hit object is item
                
                if(selection.GetComponent<SellBin>()){
                    Debug.Log("shitStorm");
                    isPerformingAction = true;
                    cashBalace = cashBalace + selection.GetComponent<SellBin>().totalValue;
                    selection.GetComponent<SellBin>().SellItems();
                    




}}

private void tryBuyVendingMachine(GameObject selection){
  
            
    
            //check if hit object is item
            
            if(selection.GetComponent<VendingMachine>()){
                Debug.Log("ahhh!");
                isPerformingAction = true;
                if(cashBalace >= selection.GetComponent<VendingMachine>().itemForSale.GetComponent<ItemInfo>().sellValue*1.5){
                    cashBalace = cashBalace - selection.GetComponent<VendingMachine>().itemForSale.GetComponent<ItemInfo>().sellValue;
                    selection.GetComponent<VendingMachine>().sellItem();
                
                }
                }

            }

private void tryPurchase(GameObject selection){


            
       
            //check if hit object is item
            
            if(selection.GetComponent<POS>()){
                if(cashBalace >= selection.GetComponent<POS>().price && !selection.GetComponent<POS>().sold){
                    cashBalace = cashBalace - selection.GetComponent<POS>().price;
                    //play cha-CHING!
                    selection.GetComponent<POS>().Sell();
                
                }
                }

            }


private void tryPlaySlotMachine(GameObject selection){
    if(selection.GetComponent<SlotMachine>()){
        selection.GetComponent<SlotMachine>().Spin(transform);
    }
}
#endregion




private void SicknessManager(){
fullScreenEffectController.setVigBlend(sickness);


       
if(sickness > 0f){
    sickness = sickness - 0.5f*Time.deltaTime;
                }


if(sickness > 5){
                
                swayCamera(sickness);

                }else{
                 cameraParent.transform.localEulerAngles = new Vector3(cameraParent.transform.localEulerAngles.x,0,cameraParent.transform.localEulerAngles.z);   
                 
                 
                }
                
if(sickness > 60){

    fullScreenEffectController.setNauseaBlend(true,sickness);
    Puke();
    delayEffect.wetMix = math.remap(60,100,0,1,sickness);
    reverbFilter.enabled= true;
    playerMovementController.GetComponent<PlayerMovementController>().pukeSpeedModifier = speedModifier;

}else{
    reverbFilter.enabled= false;
    delayEffect.wetMix = 0;
    fullScreenEffectController.setNauseaBlend(false,sickness);
    //reset run speed
    speedModifier = 1;
   
    
    
}






}
private void Puke(){
    pukeTimer = pukeTimer - Time.deltaTime;

    if(pukeTimer >7 && pukeTimer < 10){
        speedModifier= math.remap(7,10,0,1, pukeTimer);

    }
    if(pukeTimer > 6 && pukeTimer < 7){
puke.Play();
if(!pukeSound.isPlaying){
    pukeSound.Play();
    health = health - 10;
}

    }

if(pukeTimer > 3  && pukeTimer < 6){
    speedModifier = math.remap(3,6,1,0, pukeTimer);

    }


    if(pukeTimer < 0 ){
        pukeTimer = 15;
    }




}
private void swayCamera(float swayMult){

    
    swayMult = swayMult/5;
    if(swayR <= swayMult && swayDirection == true){
  
     swayR = swayR + swayMult*Time.deltaTime;
    
    }
   

    if(swayR >= -1*swayMult && swayDirection == false){
     swayR = swayR - swayMult*Time.deltaTime;



    }


  
    if(swayR >= swayMult){
        swayDirection = false;
    }

   if(swayR <= -1*swayMult){
        swayDirection = true;
    }

    cameraParent.transform.localEulerAngles = new Vector3(cameraParent.transform.localEulerAngles.x,swayR,cameraParent.transform.localEulerAngles.z);
    








}
private void trySmokeCig(bool chirality){
    setChirality(chirality);
    if(isPerformingAction == false && activeItem1.GetComponent<Cig>() ){
        if(activeItem1.GetComponent<Cig>().isSmoked == false){
        activeItem1.GetComponent<Cig>().isSmoked = true;
        isPerformingAction = true;
        activeItem1.gameObject.GetComponent<Animator>().SetBool("isSmoking", true);
        isPerformingLongAction = true;

       
        sickness = sickness + 25;
        

    
    }}

    



}
private void tryDrinkBeer(bool chirality){
//takes time, like vaping, animation, on empty can return/sell bottle at store
//need to do is preforming action thing similar to vape, is drinking or is not preforming action
    setChirality(chirality);
if(isPerformingAction == false | isDrinking == true){
    if(activeItem1.GetComponent<Drink>()){
        if(activeItem1.GetComponent<Drink>().liquidPercentage > 0){
            isDrinking = true;
            activeItem1.GetComponent<Drink>().drainLiquid();
            if(sickness > 0){

            sickness = sickness - 5*Time.deltaTime;
            }
            
        }
        
    
}


}
}
private void tryDepositCig(bool chirality){

    setChirality(chirality);


    if(isPerformingAction == false
    && activeItem1.GetComponent<CigPacket>()
    && activeItem2.GetComponent<Cig>()){
    if(activeItem1.GetComponent<CigPacket>().depositCig(activeItem2.gameObject)){
        isPerformingAction = true;
        if(chirality){

        itemRightHand = emptyItem;
    }else{

        itemLeftHand = emptyItem;
    }
    }
    
    

}
}
private void tryWithdrawlCig(bool chirality){
    
    setChirality(chirality);


    if(isPerformingAction == false
    && activeItem1.GetComponent<CigPacket>()
    && activeItem2 == emptyItem){
        GameObject cigToEquip;
        cigToEquip = activeItem1.GetComponent<CigPacket>().withdrawlCig();
    
    if(cigToEquip != null){
        isPerformingAction = true;
         cigToEquip.transform.position = activeHand2.transform.position;
        cigToEquip.transform.rotation = activeHand2.transform.rotation;
        cigToEquip.transform.parent = activeHand2.transform;
        cigToEquip.transform.localEulerAngles = cigToEquip.GetComponent<ItemInfo>().parentRotationOffset;
        cigToEquip.transform.localPosition = cigToEquip.GetComponent<ItemInfo>().parentPositionOffset;

         if(chirality){

        itemRightHand = cigToEquip.GetComponent<ItemInfo>();
    }else{

        itemLeftHand = cigToEquip.GetComponent<ItemInfo>();
    }
    }



       
    
    
    

}
}
private void tryFillPod(bool chirality){
setChirality(chirality);

if(!isPerformingAction || isFillingPod == true){
if(activeItem1.GetComponent<PodItem>() && activeItem2.GetComponent<JuiceBottle>()){
    
    if(activeItem1.GetComponent<PodItem>().isFillable){

        
        //call function to drain juice from right hand
        //call function to add juice to left hand
        if(activeItem1.GetComponent<PodItem>().juicePercentage < 100 && activeItem2.GetComponent<JuiceBottle>().juicePercentage > 0){

            activeItem2.GetComponent<JuiceBottle>().JuiceDrain();

            activeItem1.GetComponent<PodItem>().JuiceFill();

        isPerformingAction = true;
        isFillingPod = true;
        
        }





    }



}
}




}
private void tryAttachPod(bool chirality){
 //check for possible to attach pod and do so
 setChirality(chirality);
    if(isPerformingAction == false
    && activeItem1.GetComponent<DeviceItem>()
     && activeItem2.GetComponent<PodItem>()
     && !activeItem1.GetComponent<DeviceItem>().attachedPod
     && activeItem1.GetComponent<DeviceItem>().checkCompatiblity(activeItem2.GetComponent<PodItem>())){


        //call method in device to attach pod to device
        activeItem1.GetComponent<DeviceItem>().attachPod(activeItem2.GetComponent<PodItem>());
        //set hand that contained pod to isntance of empty item(null replacement)
       
        if(chirality){
        itemRightHand = emptyItem;
        }else{
        itemLeftHand = emptyItem;

        }

       
        isPerformingAction = true;
    }
}

private void tryDettachPod(bool chirality){
    setChirality(chirality);
    if(isPerformingAction == false
    && activeItem1.GetComponent<DeviceItem>() 
    && activeItem2 == emptyItem){
            //retrieve device script
            DeviceItem device = activeItem1.gameObject.GetComponent<DeviceItem>();

            //check if pod attached
            if(device.attachedPod){ 
                //retrieve pod script
                PodItem pod = device.attachedPod;
            
             //call method in device to remove pod from device
            activeItem1.GetComponent<DeviceItem>().dettachPod();




        pod.gameObject.transform.parent = activeHand2.transform;
        pod.gameObject.transform.position = activeHand2.transform.position;
        pod.gameObject.transform.rotation = activeHand2.transform.rotation;   
        if(chirality){
        itemRightHand = pod.gameObject.GetComponent<ItemInfo>();   
        }else{
        itemLeftHand = pod.gameObject.GetComponent<ItemInfo>(); 

        }
        isPerformingAction = true; 
                  
                }

                                            }
    
}

private void tryVape(bool chirality){


setChirality(chirality);
//check if able to vape
    //check if item is vape device
 if((isPerformingAction == false 
 || isVaping ==true)
 && activeItem1.GetComponent<DeviceItem>()){
        //retrieve device script
        DeviceItem device = activeItem1.gameObject.GetComponent<DeviceItem>();

        //check if pod attached
        if(device.attachedPod){ 
            //retrieve pod script
            PodItem pod = device.attachedPod;
            if(isVaping == false){
             vapeSound.Play();
              smoke.Stop();
        }
            isVaping = true;
            isPerformingAction = true;
            //check if we have juice and battery
            if(device.batteryPercentage > 0 && pod.juicePercentage > 0){
                //animate
                animator.SetBool("isChuffing?", true);
                //drain battery
                device.BatteryDrain();
                //drain juice
                pod.JuiceDrain();
                //increase Light
                device.updateLight();
                //apply sickness
                  
                if(sickness < 100f){
                    sickness = sickness + Time.deltaTime*sicknessMultiplier;
                                                    }

            }
                       
        //temp UI     
        currentPodJuicePercentage = pod.juicePercentage;
        currentDeviceBatteryPercentage = device.batteryPercentage;
            
            
            
            
            
            
            }



                                        }



}
//NEEDS ANIMATION
private void setChirality(bool chirality){
    if(chirality){
        activeHand1 = leftHand;
        activeItem1 = itemLeftHand;
        activeHand2 = rightHand;
        activeItem2 = itemRightHand;
    }else{
        activeHand1 = rightHand;
        activeItem1 = itemRightHand;
        activeHand2 = leftHand;
        activeItem2 = itemLeftHand;     
    }
}
private void tryPickUp(bool chirality){
    //factor to function
    setChirality(chirality);
    

 if(isPerformingAction == false
 && activeItem1 == emptyItem){
    //raycast    
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){

        var selection = hit.transform.gameObject;
            //check if hit object is item
            if(selection.GetComponent<ItemInfo>()){
            Debug.Log("found item!" + selection);
        //assign item to hand and transform(transform info for hand placement should be added to iteminfo script)
            


        
        if(selection.GetComponent<ItemInfo>().isHuntable){
            selection.layer = LayerMask.NameToLayer("Default");
        }
        if(selection.GetComponent<DeviceItem>()){
            selection.GetComponent<DeviceItem>().attachedCharger = false;
        }
        
        selection.GetComponent<Rigidbody>().isKinematic = true;
        selection.GetComponent<Rigidbody>().useGravity = false;
        selection.GetComponent<BoxCollider>().isTrigger = true;
        if(selection.GetComponent<ItemInfo>().isOnGround){
        selection.transform.localScale = selection.transform.localScale*.5f;
        }
        selection.GetComponent<ItemInfo>().isOnGround = false;
        selection.transform.position = activeHand1.transform.position;
        selection.transform.rotation = activeHand1.transform.rotation;
        selection.transform.parent = activeHand1.transform;
        selection.transform.localEulerAngles = selection.GetComponent<ItemInfo>().parentRotationOffset;
        selection.transform.localPosition = selection.GetComponent<ItemInfo>().parentPositionOffset;
        if(chirality){
        itemLeftHand = selection.GetComponent<ItemInfo>();
        }else{
            itemRightHand = selection.GetComponent<ItemInfo>();

        }
        isPerformingAction = true;

        if(selection.GetComponent<PickableNavAgent>()){
            selection.GetComponent<NavMeshAgent>().enabled = false;
        }
       

            }
}
}
}


private void tryPlaceInventoryLeft(){
    //check if inventory is open, check if firing

    
    //start inventory placement
if(Input.GetButton("Fire1") && inventoryOpen && !isPerformingAction && !attemptingInventoryPlacementRight){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //define raycast hit 
        RaycastHit hit;
        //send raycast
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
                //define selection    
                var selection = hit.transform.gameObject;
                    //check if selection is inventory
            if(selection.GetComponent<Inventory>()){
                    

                activeItem1 = itemLeftHand;
                activeHand1 = leftHand;
                //check if you have an item
                    if(activeItem1 != emptyItem){
                        attemptingInventoryPlacementLeft = true;
                        isPerformingAction = true;
               
                                                }
                    }}}


    if(attemptingInventoryPlacementLeft){
        //define ray 
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //define raycast hit 
        RaycastHit hit;
        //send raycast
            if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
                

            //define selection    
            var selection = hit.transform.gameObject;
                //check if selection is inventory
                if(selection.GetComponent<Inventory>()){



            //now attempting inventory placement
            ItemInfo item = activeItem1.GetComponent<ItemInfo>();   
            item.setGhostMaterial();
            activeItem1.transform.parent = selection.transform;
            activeItem1.transform.position = hit.point;
            activeItem1.transform.localEulerAngles=item.inventoryRotationOffset;


                }
    }
    }
      //if on mouse release or inventory close, if you don't have a valid spot, return item to hand
    //if on mouse relaese or inventory close, and yoiu have a valiud spot, leave item in inventory
    //if were attempting placement, and the inventory has been closed OR we stop pressing inventory button
    
    //if you close your inventory during palcement, just return item to your hand
    if(attemptingInventoryPlacementLeft && !inventoryOpen){
        attemptingInventoryPlacementLeft = false;
        activeItem1.GetComponent<ItemInfo>().restoreMaterial();
        Debug.Log("triggered inventory placement cancel");
        activeItem1.transform.position = activeHand1.transform.position;
        activeItem1.transform.rotation = activeHand1.transform.rotation;
        activeItem1.transform.parent = activeHand1.transform;
        activeItem1.transform.localEulerAngles = activeItem1.GetComponent<ItemInfo>().parentRotationOffset;
        activeItem1.transform.localPosition = activeItem1.GetComponent<ItemInfo>().parentPositionOffset;
        

    }
    
    //if you release mouse 1 check if its the right spot, if not put back into hand
    if(attemptingInventoryPlacementLeft && !Input.GetButton("Fire1")){
        Debug.Log("triggered inventory placement end");
        activeItem1.GetComponent<ItemInfo>().restoreMaterial();
        attemptingInventoryPlacementLeft = false;
        if(itemLeftHand.GetComponent<ItemInfo>().colliding){
                   
        //set back to hand
            activeItem1.transform.position = activeHand1.transform.position;
            activeItem1.transform.rotation = activeHand1.transform.rotation;
            activeItem1.transform.parent = activeHand1.transform;
            activeItem1.transform.localEulerAngles = activeItem1.GetComponent<ItemInfo>().parentRotationOffset;
            activeItem1.transform.localPosition = activeItem1.GetComponent<ItemInfo>().parentPositionOffset;
            
                //update material
        }else{
            
            itemLeftHand = emptyItem;
            //update material

        }



    }


    
}


private void tryPlaceInventoryRight(){
    //check if inventory is open, check if firing

    
    //start inventory placement
if(Input.GetButton("Fire2") && inventoryOpen && !isPerformingAction && !attemptingInventoryPlacementLeft){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //define raycast hit 
        RaycastHit hit;
        //send raycast
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
                //define selection    
                var selection = hit.transform.gameObject;
                    //check if selection is inventory
            if(selection.GetComponent<Inventory>()){
                    

                activeItem1 = itemRightHand;
                activeHand1 = rightHand;
                //check if you have an item
                    if(activeItem1 != emptyItem){
                        attemptingInventoryPlacementRight = true;
                        isPerformingAction = true;
                                                }
                    }}}


    if(attemptingInventoryPlacementRight){
        //define ray 
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //define raycast hit 
        RaycastHit hit;
        //send raycast
            if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
                

            //define selection    
            var selection = hit.transform.gameObject;
                //check if selection is inventory
                if(selection.GetComponent<Inventory>()){



            //now attempting inventory placement
          
            ItemInfo item = activeItem1.GetComponent<ItemInfo>();   
            item.setGhostMaterial();
            activeItem1.transform.parent = selection.transform;
            activeItem1.transform.position = hit.point;
            activeItem1.transform.localEulerAngles=item.inventoryRotationOffset;


                }
    }
    }
      //if on mouse release or inventory close, if you don't have a valid spot, return item to hand
    //if on mouse relaese or inventory close, and yoiu have a valiud spot, leave item in inventory
    //if were attempting placement, and the inventory has been closed OR we stop pressing inventory button
    
    //if you close your inventory during palcement, just return item to your hand
    if(attemptingInventoryPlacementRight && !inventoryOpen){
        attemptingInventoryPlacementRight = false;
        activeItem1.GetComponent<ItemInfo>().restoreMaterial();
        Debug.Log("triggered inventory placement cancel");
        activeItem1.transform.position = activeHand1.transform.position;
        activeItem1.transform.rotation = activeHand1.transform.rotation;
        activeItem1.transform.parent = activeHand1.transform;
        activeItem1.transform.localEulerAngles = activeItem1.GetComponent<ItemInfo>().parentRotationOffset;
        activeItem1.transform.localPosition = activeItem1.GetComponent<ItemInfo>().parentPositionOffset;


    }
    
    //if you release mouse 1 check if its the right spot, if not put back into hand
    if(attemptingInventoryPlacementRight && !Input.GetButton("Fire2")){
        Debug.Log("triggered inventory placement end");
        activeItem1.GetComponent<ItemInfo>().restoreMaterial();
        attemptingInventoryPlacementRight = false;
        if(itemRightHand.GetComponent<ItemInfo>().colliding){
                   
        //set back to hand
            activeItem1.transform.position = activeHand1.transform.position;
            activeItem1.transform.rotation = activeHand1.transform.rotation;
            activeItem1.transform.parent = activeHand1.transform;
            activeItem1.transform.localEulerAngles = activeItem1.GetComponent<ItemInfo>().parentRotationOffset;
            activeItem1.transform.localPosition = activeItem1.GetComponent<ItemInfo>().parentPositionOffset;
                //update material
        }else{
            
            itemRightHand = emptyItem;
            //update material

        }



    }


    
}

private void tryDropItem(bool chirality){
    setChirality(chirality);
if(isPerformingAction == false
&& activeItem1 != emptyItem){
    isPerformingAction = true;
    if(!activeItem1.GetComponent<ItemInfo>().isOnGround){
        activeItem1.transform.localScale = activeItem1.transform.localScale*2f;
        }
    activeItem1.GetComponent<ItemInfo>().isOnGround = true;
    activeItem1.GetComponent<Rigidbody>().isKinematic = false;
    activeItem1.GetComponent<Rigidbody>().useGravity = true;
    activeItem1.GetComponent<BoxCollider>().isTrigger = false;
    

    if(activeItem1.isHuntable){
            activeItem1.gameObject.layer = LayerMask.NameToLayer("roaches");
        }    

    

    activeItem1.transform.parent = null;

    if(chirality){
        itemLeftHand = emptyItem;
    }else{

        itemRightHand = emptyItem;
    }


   
    //raycast    
    /*
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){

        var selection = hit.transform.gameObject;
        

        activeItem1.transform.localEulerAngles = new Vector3(0,0,0); 
        activeItem1.transform.eulerAngles = activeItem1.inventoryRotationOffset;
        activeItem1.transform.position = hit.point;
        
        activeItem1.transform.parent = null;
        
    

        if(chirality){
        itemLeftHand = emptyItem;
        }else{
        itemRightHand = emptyItem;
        
        isPerformingAction = true;

            
}
}
*/
    
}




}

private void tryPlaceCharger(bool chirality){
    setChirality(chirality);

if(activeItem1.GetComponent<DeviceItem>() && isPerformingAction == false){
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
            
        var selection = hit.transform.gameObject;
if(selection.GetComponent<Outlet>()){
        isPerformingAction = true;
        Debug.Log("facts son");
        activeItem1.transform.position = hit.point;
        activeItem1.transform.parent = selection.transform;
        activeItem1.transform.localRotation=quaternion.identity;
        activeItem1.GetComponent<DeviceItem>().attachedCharger = true;
        

        if(chirality){
            itemLeftHand = emptyItem;
        }else{

            itemRightHand = emptyItem;
        }




    }
}
        }



}





private void tryPlaceKey(bool chirality){
    setChirality(chirality);

if(activeItem1.GetComponent<Key>() && isPerformingAction == false){
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
            
        var selection = hit.transform.gameObject;
if(selection.GetComponent<Lock>()){
        isPerformingAction = true;
        
        activeItem1.transform.position = hit.point;
        activeItem1.transform.parent = selection.transform;
        activeItem1.transform.localRotation=quaternion.identity;
        Destroy(activeItem1.GetComponent<BoxCollider>());
        selection.GetComponent<Lock>().insertKey();
        {
            
        }
        

        if(chirality){
            itemLeftHand = emptyItem;
        }else{

            itemRightHand = emptyItem;
        }




    }
}
        }



}



private void tryDepositCash(bool chirality){

    setChirality(chirality);
var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
        if(Physics.Raycast(ray, out hit,pickUpRange,RayCastHitable)){
            
        var selection = hit.transform.gameObject;
            //check if hit object is item
            
            if(selection.GetComponent<CashRegister>()){
                if(activeItem1.GetComponent<Cash>()){
Debug.Log("ping!");
cashBalace = cashBalace + activeItem1.GetComponent<Cash>().value;
selection.GetComponent<CashRegister>().chaChing.pitch = UnityEngine.Random.Range(0.9f,1.1f);
selection.GetComponent<CashRegister>().chaChing.Play();
//temp until animation is complete
if(chirality){
Destroy(itemLeftHand.gameObject);
itemLeftHand = emptyItem;
}else{
Destroy(itemRightHand.gameObject);
itemRightHand = emptyItem;

}


}

            }
}
}






}
 