using System;
using System.Collections;
using System.Net;
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
public class ActionManager : MonoBehaviour {
    #region Variables
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

    public bool isPerformingActionLeft = false;
    public bool isPerformingActionRight = false;

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

    private bool attemptingInventoryPlacementRight = false;
    [SerializeField]
    private Light inventoryLight;

    [Header("ActionManagement")]


    private bool isPerformingComboAction = false;

    public bool isVaping = false;

    private float smokeTimer = 0;



    [Header("PlayerStats")]
    public float health = 6;
    public float sickness = 0;
    public float craving = 0;
    [SerializeField]
    private float pickUpRange = 100f;

    [SerializeField]
    private float sicknessMultiplier = 1f;
    [SerializeField]
    private float cravingMultiplier = 1f;

    [SerializeField]
    private ItemInfo emptyItem;

    private bool isFillingPod = false;
    public TextMeshProUGUI statusBars;

    public float cashBalace = 0;


    [HideInInspector]
    public bool isPerformingLongAction = false;
    private float pukeTimer = 15;

    [Header("UISTATS")]
    private float currentDeviceBatteryPercentage = 100;
    private float currentPodJuicePercentage = 100;

    private float speedModifier = 1f;
    private bool isPerformingOverideLeft = false;
    private bool isPerformingOverideRight = false;
    #endregion
    void Update() {
        // Check player health
        if (health < 0) {

        }


        ResetActions();
        // Update UI

        UpdateUI();

        // Handle inventory
        HandleInventory();

        // Manage sickness
        SicknessManager();

        HandlePlayerStats();

        // Handle player actions
        HandlePlayerActions();

        // Reset actions if no buttons are pressed


        // Handle smoke timer
        HandleSmokeTimer();

        // Handle player movement Actions
        HandlePlayerMovementActions();





    }


    public void takeDamage(float damage) {
        if (health >= 0) {
            health -= damage;
        }
        else {
            //end game
        }



    }
    private void HandleInventory() {
        if (Input.GetButton("Inventory")) {
            inventory.retrieveInventroy();
            inventoryOpen = true;
            inventoryLight.enabled = true;
        }
        else {
            inventory.unRetrieveInventory();
            inventoryOpen = false;
            inventoryLight.enabled = false;
        }

        tryPlaceInventoryLeft();
        tryPlaceInventoryRight();
    }
    private void HandlePlayerStats() {
        if (!isVaping) {
            if (craving < 100) {
                craving += cravingMultiplier * Time.deltaTime;
            }
        }
        fullScreenEffectController.setAnxietyIntensity(craving);


    }
    private void HandlePlayerActions() {


        // Handle inspection
        if (Input.GetButton("Inspect")) {
            //animator.SetBool("isInspecting", true);
        }
        else {
            //animator.SetBool("isInspecting", false);
        }

        // Handle item drop
        if (Input.GetButton("DropLeft") && !isPerformingActionLeft && !isPerformingOverideLeft) {
            tryDropItem(true);
        }

        if (Input.GetButton("DropRight") && !isPerformingActionRight && !isPerformingOverideRight) {
            tryDropItem(false);
        }






        // Handle combo actions
        if (Input.GetButton("Fire1") && Input.GetButton("Fire2")) {

            //cancle any one handed actions

            PerformComboActions();
        }



        //handle single hand actions
        if (Input.GetButton("Fire1") && !isPerformingActionLeft && !isPerformingOverideLeft && !isPerformingComboAction && !attemptingInventoryPlacementLeft) {
            HandleItemUsage(true);
            tryInteract(true);
        }

        if (Input.GetButton("Fire2") && !isPerformingActionRight && !isPerformingOverideRight && !isPerformingComboAction && !attemptingInventoryPlacementRight) {
            HandleItemUsage(false);
            tryInteract(false);
        }






    }

    private void HandlePlayerMovementActions() {

        // CHARGED DASH

        /*
        The dash mechanic works by holding down the dash button , which charges it, and then on release it activates the dash.
        The dash will move the player in the direction the players camera is facing, giving velocity to the player.
        The dash has some initial velocity , and then depending on how long the player has charged the dash we will add some additional velocity.

        activation -> Movement1 : f
        */



        // Charged dash
        if (Input.GetButton("Movement1")) {
            Debug.Log("Dash button down");
            playerMovementController.isChargingDash = true;
        }
        if (Input.GetButtonDown("Movement1")) {
            playerMovementController.queueDashCharger = true;
        }
        // on release
        if (Input.GetButtonUp("Movement1")) {
            Debug.Log("Dash button released");
            playerMovementController.queueDash = true;
            playerMovementController.isChargingDash = false;
        }
        // edge case: button is up but not released

    }

    private void PerformComboActions() {

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


    }

    private void HandleItemUsage(bool isLeftHand) {
        setChirality(isLeftHand);
        //each functions should check for sucessful action and set isPerformingAction to true if action is sucessful


        tryPlaceCharger(isLeftHand);
        tryPlaceKey(isLeftHand);
        tryVape(isLeftHand);
        tryDepositCash(isLeftHand);
        tryDrinkBeer(isLeftHand);
        trySmokeCig(isLeftHand);
        tryPickUp(isLeftHand);

    }
    #region utilFunctions
    private void ResetActions() {
        if (!Input.GetButton("Fire1")) {
            isPerformingActionLeft = false;
        }
        if (!Input.GetButton("Fire2")) {
            isPerformingActionRight = false;
        }
        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2") && !isPerformingLongAction) {

            isFillingPod = false;




        }
        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2")) {
            isPerformingComboAction = false;
        }
    }

    public void setActiveAction(bool isLeftHand) {
        if (isLeftHand) {
            isPerformingActionLeft = true;
        }
        else {
            isPerformingActionRight = true;
        }
    }

    public void setActionOveride(bool isLeftHand) {
        if (isLeftHand) {
            isPerformingOverideLeft = true;
        }
        else {
            isPerformingOverideRight = true;
        }
    }
    public void releaseActionOveride(bool isLeftHand) {
        if (isLeftHand) {
            isPerformingOverideLeft = false;
        }
        else {
            isPerformingOverideRight = false;
        }
    }

    private void HandleSmokeTimer() {
        if (smokeTimer >= 0) {
            smokeTimer -= Time.deltaTime;
        }

        if (smokeTimer <= 0) {
            smoke.Stop();
        }
    }

    private void setChirality(bool isLeftHand) {
        if (isLeftHand) {
            activeHand1 = leftHand;
            activeItem1 = itemLeftHand;
            activeHand2 = rightHand;
            activeItem2 = itemRightHand;
        }
        else {
            activeHand1 = rightHand;
            activeItem1 = itemRightHand;
            activeHand2 = leftHand;
            activeItem2 = itemLeftHand;
        }
    }
    #endregion
    #region Interact Functions
    public void tryInteract(bool isLeftHand) {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
            var selection = hit.transform.gameObject;
            tryActivateButton(selection, isLeftHand);
            tryOpenLocker(selection, isLeftHand);
            trySellItems(selection, isLeftHand);
            tryBuyVendingMachine(selection, isLeftHand);
            tryPurchase(selection, isLeftHand);
            tryPlaySlotMachine(selection, isLeftHand);
        }

    }


    public void tryActivateButton(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<GameButton>()) {
            setActiveAction(isLeftHand);
            selection.GetComponent<GameButton>().activate();
        }
    }

    private void tryOpenLocker(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<LockerDoor>()) {
            setActiveAction(isLeftHand);

            selection.GetComponent<LockerDoor>().openClose();
        }
    }

    private void trySellItems(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<SellBin>()) {

            setActiveAction(isLeftHand);
            cashBalace += selection.GetComponent<SellBin>().totalValue;
            selection.GetComponent<SellBin>().SellItems();
        }
    }

    private void tryBuyVendingMachine(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<VendingMachine>()) {


            if (cashBalace >= selection.GetComponent<VendingMachine>().itemForSale.GetComponent<ItemInfo>().sellValue * 1.5) {
                setActiveAction(isLeftHand);
                cashBalace -= selection.GetComponent<VendingMachine>().itemForSale.GetComponent<ItemInfo>().sellValue;
                selection.GetComponent<VendingMachine>().sellItem();
            }
        }
    }

    private void tryPurchase(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<POS>()) {
            if (cashBalace >= selection.GetComponent<POS>().price && !selection.GetComponent<POS>().sold) {
                setActiveAction(isLeftHand);
                cashBalace -= selection.GetComponent<POS>().price;
                selection.GetComponent<POS>().Sell();
            }
        }
    }

    private void tryPlaySlotMachine(GameObject selection, bool isLeftHand) {
        if (selection.GetComponent<SlotMachine>()) {
            setActiveAction(isLeftHand);
            selection.GetComponent<SlotMachine>().Spin(transform);
        }
    }
    #endregion




    #region ItemActions
    //make courtine
    private void tryDrinkBeer(bool isLeftHand) {
        setChirality(isLeftHand);

        if (activeItem1.GetComponent<Drink>()) {
            if (activeItem1.GetComponent<Drink>().liquidPercentage > 0) {
                setActiveAction(isLeftHand);
                StartCoroutine(countineDrinkBeer(isLeftHand, activeItem1));
            }
        }

    }

    IEnumerator countineDrinkBeer(bool isLeftHand, ItemInfo beer) {

        while (!isPerformingComboAction && isLeftHand ? !attemptingInventoryPlacementLeft : !attemptingInventoryPlacementRight) {
            if (beer.GetComponent<Drink>().liquidPercentage <= 0 || isLeftHand ? !Input.GetButton("Fire1") : !Input.GetButton("Fire2")) {
                break;
            }
            if (beer.GetComponent<Drink>().liquidPercentage > 0 && isLeftHand ? Input.GetButton("Fire1") : Input.GetButton("Fire2")) {
                if (isLeftHand) {
                    animator.SetBool("IsVapingLeft?", true);
                }
                else {
                    animator.SetBool("IsVapingRight?", true);
                }
                beer.GetComponent<Drink>().drainLiquid();
                if (sickness > 0) {
                    sickness = sickness - 5 * Time.deltaTime;
                }
            }

            yield return null;

        }
        if (isLeftHand) {
            animator.SetBool("IsVapingLeft?", false);
        }
        else {
            animator.SetBool("IsVapingRight?", false);
        }

    }





    private void trySmokeCig(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1.GetComponent<Cig>()) {
            if (activeItem1.GetComponent<Cig>().isSmoked == false) {
                activeItem1.GetComponent<Cig>().isSmoked = true;
                setActiveAction(isLeftHand);
                setActionOveride(isLeftHand);
                activeItem1.gameObject.GetComponent<Animator>().SetBool("isSmoking", true);
                StartCoroutine(SmokeCig(activeItem1, isLeftHand));

                sickness = sickness + 25;
            }
        }
    }

    IEnumerator SmokeCig(ItemInfo cig, bool IsLeftHand) {

        yield return new WaitForSeconds(3);
        releaseActionOveride(IsLeftHand);

    }




    private void tryVape(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1.GetComponent<DeviceItem>()) {
            DeviceItem device = activeItem1.GetComponent<DeviceItem>();
            if (device.attachedPod) {
                PodItem pod = device.attachedPod;


                setActiveAction(isLeftHand);
                StartCoroutine(continueVaping(device, pod, isLeftHand));


            }
        }
    }


    IEnumerator continueVaping(DeviceItem device, PodItem pod, bool isLeftHand) {

        // if (Input.GetButton("Movement1")) {
        //     Debug.Log("Dash button down");
        //     playerMovementController.isChargingDash = true;
        // }
        // if(Input.GetButtonDown("Movement1")){
        //     playerMovementController.queueDashCharger = true;
        // }
        // // on release
        // if (Input.GetButtonUp("Movement1")) {
        //     Debug.Log("Dash button released");
        //     playerMovementController.queueDash = true;
        //     playerMovementController.isChargingDash = false;
        // }

        playerMovementController.queueDashCharger = true;



        while (!isPerformingComboAction && isLeftHand ? !attemptingInventoryPlacementLeft : !attemptingInventoryPlacementRight) {


            isVaping = true;
            if (device.batteryPercentage <= 0 || pod.juicePercentage <= 0 || (isLeftHand ? !Input.GetButton("Fire1") : !Input.GetButton("Fire2"))) {



                break;
            }
            if (device.batteryPercentage > 0 && pod.juicePercentage > 0) {
                playerMovementController.isChargingDash = true;

                if (!vapeSound.isPlaying) {
                    vapeSound.Play();
                }
                if (isLeftHand) {
                    animator.SetBool("IsVapingLeft?", true);
                }
                else {
                    animator.SetBool("IsVapingRight?", true);
                }

                device.BatteryDrain();
                pod.JuiceDrain();
                device.updateLight();

                if (sickness < 100f) {
                    sickness = sickness + Time.deltaTime * sicknessMultiplier;
                }

                if (craving > 0) {
                    craving -= 7 * Time.deltaTime;
                }




                //UI
                currentPodJuicePercentage = pod.juicePercentage;
                currentDeviceBatteryPercentage = device.batteryPercentage;
                yield return null;
            }


        }

        //Vaping Action Ends

        playerMovementController.queueDash = true;
        playerMovementController.isChargingDash = false;



        isVaping = false;
        vapeSound.Stop();
        if (isLeftHand) {
            animator.SetBool("IsVapingLeft?", false);
        }
        else {
            animator.SetBool("IsVapingRight?", false);
        }
        smokeTimer = 2;






    }


    private void tryPickUp(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1 == emptyItem) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<ItemInfo>()) {
                    setActiveAction(isLeftHand);
                    Debug.Log("found item!" + selection);
                    if (selection.GetComponent<ItemInfo>().isHuntable) {
                        selection.layer = LayerMask.NameToLayer("Default");
                    }
                    if (selection.GetComponent<DeviceItem>()) {
                        selection.GetComponent<DeviceItem>().attachedCharger = false;
                    }
                    selection.GetComponent<Rigidbody>().isKinematic = true;
                    selection.GetComponent<Rigidbody>().useGravity = false;
                    selection.GetComponent<BoxCollider>().isTrigger = true;
                    if (selection.GetComponent<ItemInfo>().isOnGround) {
                        selection.transform.localScale = selection.transform.localScale * .5f;
                    }
                    selection.GetComponent<ItemInfo>().isOnGround = false;
                    selection.transform.position = activeHand1.transform.position;
                    selection.transform.rotation = activeHand1.transform.rotation;
                    selection.transform.parent = activeHand1.transform;
                    selection.transform.localEulerAngles = selection.GetComponent<ItemInfo>().parentRotationOffset;
                    selection.transform.localPosition = selection.GetComponent<ItemInfo>().parentPositionOffset;
                    if (isLeftHand) {
                        itemLeftHand = selection.GetComponent<ItemInfo>();
                    }
                    else {
                        itemRightHand = selection.GetComponent<ItemInfo>();
                    }

                    if (selection.GetComponent<PickableNavAgent>()) {
                        selection.GetComponent<NavMeshAgent>().enabled = false;
                    }
                }
            }
        }
    }

    private void tryPlaceInventoryLeft() {
        if (Input.GetButton("Fire1") && inventoryOpen && !isPerformingActionLeft && !attemptingInventoryPlacementRight && !isPerformingOverideLeft && !isPerformingComboAction && !attemptingInventoryPlacementLeft) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Inventory>()) {
                    activeItem1 = itemLeftHand;
                    activeHand1 = leftHand;
                    if (itemLeftHand != emptyItem) {
                        attemptingInventoryPlacementLeft = true;
                        isPerformingActionLeft = true;
                    }
                }
            }
        }

        if (attemptingInventoryPlacementLeft) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Inventory>()) {
                    ItemInfo item = itemLeftHand.GetComponent<ItemInfo>();
                    item.setGhostMaterial();
                    itemLeftHand.transform.parent = selection.transform;
                    itemLeftHand.transform.position = hit.point;
                    itemLeftHand.transform.localEulerAngles = item.inventoryRotationOffset;
                }
            }
        }

        if (attemptingInventoryPlacementLeft && !inventoryOpen) {
            attemptingInventoryPlacementLeft = false;
            itemLeftHand.GetComponent<ItemInfo>().restoreMaterial();
            Debug.Log("triggered inventory placement cancel");
            itemLeftHand.transform.position = leftHand.transform.position;
            itemLeftHand.transform.rotation = leftHand.transform.rotation;
            itemLeftHand.transform.parent = leftHand.transform;
            itemLeftHand.transform.localEulerAngles = itemLeftHand.GetComponent<ItemInfo>().parentRotationOffset;
            itemLeftHand.transform.localPosition = itemLeftHand.GetComponent<ItemInfo>().parentPositionOffset;
        }

        if (attemptingInventoryPlacementLeft && !Input.GetButton("Fire1")) {
            Debug.Log("triggered inventory placement end");
            itemLeftHand.GetComponent<ItemInfo>().restoreMaterial();
            attemptingInventoryPlacementLeft = false;
            if (itemLeftHand.GetComponent<ItemInfo>().colliding) {
                itemLeftHand.transform.position = leftHand.transform.position;
                itemLeftHand.transform.rotation = leftHand.transform.rotation;
                itemLeftHand.transform.parent = leftHand.transform;
                itemLeftHand.transform.localEulerAngles = itemLeftHand.GetComponent<ItemInfo>().parentRotationOffset;
                itemLeftHand.transform.localPosition = itemLeftHand.GetComponent<ItemInfo>().parentPositionOffset;
            }
            else {
                itemLeftHand = emptyItem;
            }
        }
    }

    private void tryPlaceInventoryRight() {
        if (Input.GetButton("Fire2") && inventoryOpen && !isPerformingActionRight && !attemptingInventoryPlacementRight && !isPerformingOverideRight && !isPerformingComboAction && !attemptingInventoryPlacementLeft) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Inventory>()) {

                    if (itemRightHand != emptyItem) {
                        attemptingInventoryPlacementRight = true;
                        isPerformingActionRight = true;
                    }
                }
            }
        }


        if (attemptingInventoryPlacementRight) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Inventory>()) {
                    ItemInfo item = itemRightHand.GetComponent<ItemInfo>();
                    item.setGhostMaterial();
                    itemRightHand.transform.parent = selection.transform;
                    itemRightHand.transform.position = hit.point;
                    itemRightHand.transform.localEulerAngles = itemRightHand.inventoryRotationOffset;
                }
            }
        }

        if (attemptingInventoryPlacementRight && !inventoryOpen) {
            attemptingInventoryPlacementRight = false;
            itemRightHand.GetComponent<ItemInfo>().restoreMaterial();
            Debug.Log("triggered inventory placement cancel");
            itemRightHand.transform.position = rightHand.transform.position;
            itemRightHand.transform.rotation = rightHand.transform.rotation;
            itemRightHand.transform.parent = rightHand.transform;
            itemRightHand.transform.localEulerAngles = itemRightHand.GetComponent<ItemInfo>().parentRotationOffset;
            itemRightHand.transform.localPosition = itemRightHand.GetComponent<ItemInfo>().parentPositionOffset;
        }

        if (attemptingInventoryPlacementRight && !Input.GetButton("Fire2")) {
            Debug.Log("triggered inventory placement end");
            itemRightHand.GetComponent<ItemInfo>().restoreMaterial();
            attemptingInventoryPlacementRight = false;
            if (itemRightHand.GetComponent<ItemInfo>().colliding) {
                itemRightHand.transform.position = rightHand.transform.position;
                itemRightHand.transform.rotation = rightHand.transform.rotation;
                itemRightHand.transform.parent = rightHand.transform;
                itemRightHand.transform.localEulerAngles = itemRightHand.GetComponent<ItemInfo>().parentRotationOffset;
                itemRightHand.transform.localPosition = itemRightHand.GetComponent<ItemInfo>().parentPositionOffset;
            }
            else {
                itemRightHand = emptyItem;
            }
        }
    }

    private void tryDropItem(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1 != emptyItem) {
            setActiveAction(isLeftHand);
            setActionOveride(isLeftHand);
            StartCoroutine(dropItem(isLeftHand, activeItem1));


        }
    }

    IEnumerator dropItem(bool isLeftHand, ItemInfo activeItem1) {
        float throwTimer = 0;
        while (isLeftHand ? Input.GetButton("DropLeft") : Input.GetButton("DropRight")) {
            setActionOveride(isLeftHand);
            if (isLeftHand) {
                animator.SetBool("IsDropLeft", true);
            }
            else {
                animator.SetBool("IsDropRight", true);
            }
            throwTimer += Time.deltaTime;


            yield return null;
        }


        //rescale item
        if (!activeItem1.GetComponent<ItemInfo>().isOnGround) {
            activeItem1.transform.localScale = activeItem1.transform.localScale * 2f;
        }

        if (activeItem1.isHuntable) {
            activeItem1.gameObject.layer = LayerMask.NameToLayer("roaches");
        }

        //apply throw velocity
        if (throwTimer > 1.67) {
            if (isLeftHand) {
                animator.SetBool("IsDropLeft", false);
                animator.SetBool("IsThrowLeft", true);

            }
            else {
                animator.SetBool("IsDropRight", false);
                animator.SetBool("IsThrowRight", true);
            }
            yield return new WaitForSeconds(0.05f);
            activeItem1.GetComponent<ItemInfo>().isOnGround = true;
            activeItem1.GetComponent<Rigidbody>().isKinematic = false;
            activeItem1.GetComponent<Rigidbody>().useGravity = true;
            activeItem1.GetComponent<BoxCollider>().isTrigger = false;
            activeItem1.transform.parent = null;
            activeItem1.GetComponent<Rigidbody>().velocity += Camera.main.transform.forward * 10;
            if (isLeftHand) {
                animator.SetBool("IsThrowLeft", false);


            }
            else {
                animator.SetBool("IsThrowRight", false);

            }
        }
        else {
            if (isLeftHand) {
                animator.SetBool("IsDropLeft", false);


            }
            else {
                animator.SetBool("IsDropRight", false);

            }
            activeItem1.GetComponent<ItemInfo>().isOnGround = true;
            activeItem1.GetComponent<Rigidbody>().isKinematic = false;
            activeItem1.GetComponent<Rigidbody>().useGravity = true;
            activeItem1.GetComponent<BoxCollider>().isTrigger = false;
            activeItem1.transform.parent = null;
        }




        if (isLeftHand) {
            itemLeftHand = emptyItem;
        }
        else {
            itemRightHand = emptyItem;
        }
        releaseActionOveride(isLeftHand);
        //drop the item



    }

    private void tryPlaceCharger(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1.GetComponent<DeviceItem>()) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Outlet>()) {
                    setActiveAction(isLeftHand);

                    activeItem1.transform.position = hit.point;
                    activeItem1.transform.parent = selection.transform;
                    activeItem1.transform.localRotation = quaternion.identity;
                    activeItem1.GetComponent<DeviceItem>().attachedCharger = true;

                    if (isLeftHand) {
                        itemLeftHand = emptyItem;
                    }
                    else {
                        itemRightHand = emptyItem;
                    }
                }
            }
        }
    }

    private void tryPlaceKey(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1.GetComponent<Key>()) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
                var selection = hit.transform.gameObject;
                if (selection.GetComponent<Lock>()) {
                    setActiveAction(isLeftHand);

                    activeItem1.transform.position = hit.point;
                    activeItem1.transform.parent = selection.transform;
                    activeItem1.transform.localRotation = quaternion.identity;
                    Destroy(activeItem1.GetComponent<BoxCollider>());
                    selection.GetComponent<Lock>().insertKey();

                    if (isLeftHand) {
                        itemLeftHand = emptyItem;
                    }
                    else {
                        itemRightHand = emptyItem;
                    }
                }
            }
        }
    }

    private void tryDepositCash(bool isLeftHand) {
        setChirality(isLeftHand);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickUpRange, RayCastHitable)) {
            var selection = hit.transform.gameObject;
            if (selection.GetComponent<CashRegister>()) {
                if (activeItem1.GetComponent<Cash>()) {
                    setActiveAction(isLeftHand);

                    cashBalace = cashBalace + activeItem1.GetComponent<Cash>().value;
                    selection.GetComponent<CashRegister>().chaChing.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    selection.GetComponent<CashRegister>().chaChing.Play();
                    if (isLeftHand) {
                        Destroy(itemLeftHand.gameObject);
                        itemLeftHand = emptyItem;
                    }
                    else {
                        Destroy(itemRightHand.gameObject);
                        itemRightHand = emptyItem;
                    }
                }
            }
        }
    }


    #endregion

    #region comboActions

    private void tryDepositCig(bool isLeftHand) {
        setChirality(isLeftHand);
        if (activeItem1.GetComponent<CigPacket>() && activeItem2.GetComponent<Cig>() && !isPerformingComboAction) {
            if (activeItem1.GetComponent<CigPacket>().depositCig(activeItem2.gameObject)) {
                isPerformingComboAction = true;
                if (isLeftHand) {
                    itemRightHand = emptyItem;
                }
                else {
                    itemLeftHand = emptyItem;
                }
            }
        }
    }

    private void tryWithdrawlCig(bool isLeftHand) {
        setChirality(isLeftHand);
        if (isPerformingComboAction == false && activeItem1.GetComponent<CigPacket>() && activeItem2 == emptyItem) {
            GameObject cigToEquip;
            cigToEquip = activeItem1.GetComponent<CigPacket>().withdrawlCig();

            if (cigToEquip != null) {
                isPerformingComboAction = true;
                cigToEquip.transform.position = activeHand2.transform.position;
                cigToEquip.transform.rotation = activeHand2.transform.rotation;
                cigToEquip.transform.parent = activeHand2.transform;
                cigToEquip.transform.localEulerAngles = cigToEquip.GetComponent<ItemInfo>().parentRotationOffset;
                cigToEquip.transform.localPosition = cigToEquip.GetComponent<ItemInfo>().parentPositionOffset;

                if (isLeftHand) {
                    itemRightHand = cigToEquip.GetComponent<ItemInfo>();
                }
                else {
                    itemLeftHand = cigToEquip.GetComponent<ItemInfo>();
                }
            }
        }
    }

    private void tryFillPod(bool isLeftHand) {
        setChirality(isLeftHand);
        if (!isPerformingComboAction) {
            if (activeItem1.GetComponent<PodItem>() && activeItem2.GetComponent<JuiceBottle>()) {
                if (activeItem1.GetComponent<PodItem>().isFillable) {
                    StartCoroutine(countinueFillPod(isLeftHand));
                    isPerformingComboAction = true;
                }
            }
        }
    }

    IEnumerator countinueFillPod(bool isLeftHand) {


        while (!isPerformingComboAction && isLeftHand ? !attemptingInventoryPlacementLeft : !attemptingInventoryPlacementRight) {
            if (activeItem1.GetComponent<PodItem>().juicePercentage >= 100 || activeItem2.GetComponent<JuiceBottle>().juicePercentage <= 0 || isLeftHand ? !Input.GetButton("Fire1") : !Input.GetButton("Fire2")) {

                break;
            }
            if (activeItem1.GetComponent<PodItem>().juicePercentage < 100 && activeItem2.GetComponent<JuiceBottle>().juicePercentage > 0) {
                activeItem2.GetComponent<JuiceBottle>().JuiceDrain();
                activeItem1.GetComponent<PodItem>().JuiceFill();


                yield return null;
            }
        }

    }

    private void tryAttachPod(bool isLeftHand) {
        setChirality(isLeftHand);
        if (isPerformingComboAction == false && activeItem1.GetComponent<DeviceItem>() && activeItem2.GetComponent<PodItem>() && !activeItem1.GetComponent<DeviceItem>().attachedPod && activeItem1.GetComponent<DeviceItem>().checkCompatiblity(activeItem2.GetComponent<PodItem>())) {
            activeItem1.GetComponent<DeviceItem>().attachPod(activeItem2.GetComponent<PodItem>());
            if (isLeftHand) {
                itemRightHand = emptyItem;
            }
            else {
                itemLeftHand = emptyItem;
            }
            isPerformingComboAction = true;
        }
    }

    private void tryDettachPod(bool isLeftHand) {
        setChirality(isLeftHand);
        if (!isPerformingComboAction && activeItem1.GetComponent<DeviceItem>() && activeItem2 == emptyItem) {
            DeviceItem device = activeItem1.gameObject.GetComponent<DeviceItem>();
            if (device.attachedPod) {
                PodItem pod = device.attachedPod;
                activeItem1.GetComponent<DeviceItem>().dettachPod();
                pod.gameObject.transform.parent = activeHand2.transform;
                pod.gameObject.transform.position = activeHand2.transform.position;
                pod.gameObject.transform.rotation = activeHand2.transform.rotation;
                if (isLeftHand) {
                    itemRightHand = pod.gameObject.GetComponent<ItemInfo>();
                }
                else {
                    itemLeftHand = pod.gameObject.GetComponent<ItemInfo>();
                }
                isPerformingComboAction = true;
            }
        }
    }



    #endregion

    #region otherFunctions
    private void SicknessManager() {
        fullScreenEffectController.setVigBlend(sickness);

        if (sickness > 0f) {
            sickness -= 0.5f * Time.deltaTime;
        }

        if (sickness > 5) {
            swayCamera(sickness);
        }
        else {
            cameraParent.transform.localEulerAngles = new Vector3(cameraParent.transform.localEulerAngles.x, 0, cameraParent.transform.localEulerAngles.z);
        }

        if (sickness > 60) {
            fullScreenEffectController.setNauseaBlend(true, sickness);
            Puke();
            delayEffect.wetMix = math.remap(60, 100, 0, 1, sickness);
            reverbFilter.enabled = true;
            playerMovementController.GetComponent<PlayerMovementController>().pukeSpeedModifier = speedModifier;
        }
        else {
            reverbFilter.enabled = false;
            delayEffect.wetMix = 0;
            fullScreenEffectController.setNauseaBlend(false, sickness);
            speedModifier = 1;
        }
    }

    private void Puke() {
        pukeTimer -= Time.deltaTime;

        if (pukeTimer > 7 && pukeTimer < 10) {
            speedModifier = math.remap(7, 10, 0, 1, pukeTimer);
        }

        if (pukeTimer > 6 && pukeTimer < 7) {
            puke.Play();
            if (!pukeSound.isPlaying) {
                pukeSound.Play();
                health -= 1;
            }
        }

        if (pukeTimer > 3 && pukeTimer < 6) {
            speedModifier = math.remap(3, 6, 1, 0, pukeTimer);
        }

        if (pukeTimer < 0) {
            pukeTimer = 15;
        }
    }

    private void swayCamera(float swayMult) {
        swayMult /= 5;
        if (swayR <= swayMult && swayDirection) {
            swayR += swayMult * Time.deltaTime;
        }

        if (swayR >= -swayMult && !swayDirection) {
            swayR -= swayMult * Time.deltaTime;
        }

        if (swayR >= swayMult) {
            swayDirection = false;
        }

        if (swayR <= -swayMult) {
            swayDirection = true;
        }

        cameraParent.transform.localEulerAngles = new Vector3(cameraParent.transform.localEulerAngles.x, swayR, cameraParent.transform.localEulerAngles.z);
    }
    private void UpdateUI() {
        statusBars.text =
            "Juice" + (int)currentPodJuicePercentage +
            "<br>Battery" + (int)currentDeviceBatteryPercentage +
            "<br>Sickness" + (int)(sickness) +
            "<br>Craving" + (int)(craving) +
            "<br>Health " + (int)(health);
    }
    #endregion

}
