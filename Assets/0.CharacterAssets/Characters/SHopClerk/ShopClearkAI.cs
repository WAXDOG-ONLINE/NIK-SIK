using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopClearkAI : MonoBehaviour {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private ParticleSystem smoke;

    [SerializeField]
    private AudioSource clerkVapeSound;
    private bool hasTime = true;
    private float chooseTimer;
    private float chuffTimer = 6f;
    private float smokeTimer = 0f;


    private bool chuffing;
    // Start is called before the first frame update
    void Start() {
        chooseTimer = 2f;
    }

    // Update is called once per frame
    void Update() {

        if (hasTime) {
            if (chooseTimer >= 0) {
                chooseTimer = chooseTimer - 1 * Time.deltaTime;
            }
            if (chooseTimer <= 0) {

                int choice = Random.Range(1, 3);
                if (choice == 1) {
                    //play animation
                    chuffing = true;
                }
                if (choice == 2) {
                    Walk();
                }
                hasTime = false;


            }



        }

        if (!hasTime) {


            chooseTimer = Random.Range(15, 30);
            hasTime = true;

        }

        ChuffAnim();
        SmokeAnim();




    }

    private void Walk() {

        GetComponent<ClerkMovement>().backToStart = false;



    }
    private void ChuffAnim() {
        if (chuffing == true) {
            chuffTimer = 6f;
            animator.SetBool("isChuffing", true);
            chuffing = false;
            clerkVapeSound.Play();
        }
        if (chuffTimer >= -1) {

            chuffTimer = chuffTimer - Time.deltaTime;
        }
        if (chuffTimer <= 0 && chuffTimer > -0.5) {
            clerkVapeSound.Stop();
            animator.SetBool("isChuffing", false);
            smokeTimer = 3f;


        }



    }
    private void SmokeAnim() {



        if (smokeTimer == 3f) {
            smoke.Play();



        }
        if (smokeTimer >= 0f) {
            smokeTimer = smokeTimer - Time.deltaTime;



        }
        if (smokeTimer <= 0f) {
            smoke.Stop();
        }


    }
}
