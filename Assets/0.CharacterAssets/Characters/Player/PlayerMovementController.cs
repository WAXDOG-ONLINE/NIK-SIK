using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("FPSCONTROLLER")]
    public float lookSpeed;
    public float lookXLimit = 45f;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 17;
    public bool canMove = true;
    
    [Header("PlayerMovement")]
    [SerializeField]
    private float initialDashStrength = 10f;
    [SerializeField]
    private float maxChargeDashStrength = 15f;
    [Tooltip("second")]
    [SerializeField]
    private float dashChargeTime = 1.0f;
    // charge curve
    [SerializeField]
    private AnimationCurve chargeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private float dashTime = 0.25f;
    [SerializeField]
    private int maxDashes = 1; // max dashs while in air
    private int dashCount = 0;
    // private float currentDashCharge = 0;
    public float currentDashChargeTime = 0.0f;
    public float currentDashCharge = 0.0f;
    public bool isChargingDash = false;
    public bool queueDash = false;
    private float initCamFov;


    public bool queueDashCharger = false;
    private float dashTimer = 0;

    CharacterController characterController;

    public Camera playerCamera;

   

    public float walkSpeed;
    public float runSpeed;
    public float groundFriction = 1.0f;
    public float airFriction = 0.5f;
    public float startWalkSpeed = 6f;
    public float startRunSpeed = 12f;
    public float startLookSpeed = 2f;
    public float jumpPower = 7f;

    public float airControlBlend = 0;
    public float gravity = 10f;

    public float pukeSpeedModifier = 1;

    public float goopSpeedModifier = 1;

    private Vector3 dashDirection = new Vector3(0,0,0);
    public AudioSource walkSound;
    public AudioSource runSound;

    // Start is called before the first frame update
    void Start()
    {
         characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        walkSpeed = startWalkSpeed;
        runSpeed = startRunSpeed;
        lookSpeed = startLookSpeed;
        initCamFov = playerCamera.fieldOfView;
        
    }

    // Update is called once per frame
    void Update()
    {
         
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        
 
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0; // Modify this line
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; // Modify this line
        float movementDirectionY = moveDirection.y;

        if (characterController.isGrounded)
        {
            moveDirection = ((forward * curSpeedX) + (right * curSpeedY))*pukeSpeedModifier*goopSpeedModifier;
        }
        else
        {
            // Blend input direction with current velocity to add a small amount of control
            Vector3 inputDirection = ((forward * curSpeedX) + (right * curSpeedY));
            moveDirection = Vector3.Lerp(moveDirection, inputDirection, airControlBlend); 
        }
 
       if(characterController.isGrounded && characterController.velocity.magnitude > 0 ){
        if(isRunning){
            if(!runSound.isPlaying){
                runSound.Play();
            }
            
        walkSound.Stop();
        }else{
            if(!walkSound.isPlaying){
                walkSound.Play();
            }
            runSound.Stop();
        
        }
        
       }else{
          runSound.Stop();
        walkSound.Stop();
       }
 
      
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower*goopSpeedModifier;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // DASH
        if (characterController.isGrounded) { dashCount = 0; }

        bool canDash = dashCount < maxDashes && !(dashTimer > 0);
        
        if(queueDashCharger){
            currentDashChargeTime = 0;
            queueDashCharger = false; 
        }





        if (isChargingDash && canDash) { ChargeDash(); }
        Vector3 dashForce = Vector3.zero;
        float dashSpeed = initialDashStrength + currentDashCharge;
        if (queueDash && !canDash) { queueDash = false; }
        if (queueDash && canDash) { 
            Vector3 curDashDir = moveDirection.normalized;
            // if the player not moving left or right, dash in the direction the player is facing
            if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0) { curDashDir = transform.forward; }



            Debug.Log("DASHED");
            // set vertical component to 0
            curDashDir.y = 0.0f;
            dashDirection = curDashDir;
            dashTimer += dashTime;
            dashCount++;
            dashForce = dashDirection * dashSpeed;
            queueDash = false;
            playerCamera.fieldOfView = initCamFov;
        } 
        dashTimer -= Time.deltaTime;
        dashTimer = Mathf.Max(dashTimer, 0);
        // if (dashTimer > 0) { 
        //   Debug.Log("DASHING");
        //   dashForce = dashDirection * dashSpeed; 
        // }


        Vector3 prevFrameVelocity = characterController.velocity;
        Vector3 playerMoveForce = moveDirection;

        //FRICTION
        Vector3 frictionForce = Vector3.zero;
        float isGrounded = characterController.isGrounded ? 1.0f : 0.0f;
        float floorDrag = isGrounded * groundFriction;
        float airDrag = airFriction;
        float frictionCoeff = floorDrag + airDrag;
        frictionCoeff *= Time.deltaTime; // Scale by time to make it frame rate independent
        frictionCoeff = Mathf.Min(frictionCoeff, 1.0f); // Clamp to 1.0f, so we dont ever move backwards, we only want to stop movement, not reverse it
        frictionForce = -prevFrameVelocity * frictionCoeff;

        //Net Force
        Vector3 netForce = Vector3.zero
         + playerMoveForce 
         + dashForce 
         + frictionForce;
        
        float mass = 1.0f;
        Vector3 acceleration = netForce / mass;
        Vector3 newVelocity = prevFrameVelocity + acceleration;

        // DISPLACEMENT INTEGRATION
        /*
        To get displacement we entegrate the velocity over time
        inbetween the previous frame and the current frame, we dont know the velocity is exactly
        so we assume a linear interpolation between the previous frame velocity and the new velocity
        the integration of this linear interpolation can be appoximated by taking the midpoint of the two velocities
        */
        Vector3 displacement = (prevFrameVelocity + newVelocity) * 0.5f * Time.deltaTime; // midpoint integration
        characterController.Move(displacement);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

 
        
    }

    public void ChargeDash(){
        /*
        CHARGEDASH:
        Charges the dash by increasing the currentDashCharge by dashChargeRate per second
        */
        Debug.Log("CHARGING DASH");
        currentDashChargeTime += Time.deltaTime;
        currentDashChargeTime = Mathf.Min(currentDashChargeTime, dashChargeTime);
        float chargePercent = currentDashChargeTime / (dashChargeTime + 0.0001f); // avoid divide by zero

        //modify the fov based on the charge percent
        float minFov = initCamFov;
        float addedFov = 20;
        float maxFov = initCamFov + addedFov;
        float fov = chargeCurve.Evaluate(chargePercent) * (maxFov - minFov) + minFov;
        playerCamera.fieldOfView = fov;

        float chargeRange = maxChargeDashStrength - initialDashStrength;
        currentDashCharge = chargeCurve.Evaluate(chargePercent) * chargeRange + initialDashStrength;






    }


}
