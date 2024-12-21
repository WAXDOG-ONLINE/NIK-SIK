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

     CharacterController characterController;

      public Camera playerCamera;

   

    public float walkSpeed;
    public float runSpeed;
    public float startWalkSpeed = 6f;
    public float startRunSpeed = 12f;
    public float startLookSpeed = 2f;
    public float jumpPower = 7f;

    public float airControlBlend = 0;
    public float gravity = 10f;

    public float pukeSpeedModifier = 1;

    public float goopSpeedModifier = 1;
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
        
        
        
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
 
        
    }
}
