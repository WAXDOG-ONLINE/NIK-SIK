using System;
using System.Collections;
using System.Collections.Generic;


using UnityEditor.Callbacks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations; //important

//if you use this code you are contractually obligated to like the YT video
public class EnemyAI : MonoBehaviour //don't forget to change the script name if you haven't
{
    
    //SOUND EFFECTS
    public AudioSource beastBreathing;
    public AudioSource beastIntChase;

    private Color debugStateColor = Color.gray;
    
    public AudioSource beastRun;

    public AudioSource beastWalk;

    public AudioSource beastChase;
    
    public Animator beastAnim;

    //list of possible roaches 1 is the max
     GameObject[] roaches = new GameObject[1];
    //target Roach
    public GameObject roach;

    public GameObject emptyObject;
    public GameObject huntedRoach;

    public LayerMask whatIsPlayer;

    public GameObject player;

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area



    //Nav ADD SENSOR CIRCLE SMALL RADIUS AROUDN GUY
    public BeastVision beastVision;

    public BeastCloseVision BeastCloseVision;

    public NavMeshAgent agent;

    
    
    



    

    public bool playerInSightRange;

    public bool roachInSightRange;

    public bool roachInSightRangePrevious;

    //Chase params

    public float chaseTimerMax;
    private bool chaseActive = false; 
    private bool chaseEndTimerActive = false;
    private float chaseTimer = 0;
    public float chaseSpeed = 6f;
    
    
    //random walk params
    public float walkSpeed;
    public float range; //radius of sphere



    //Idle params
    public bool idleing = false;
  
    public float maxIdleTime = 4;

    //other
    private bool playerInSightRangePrevious;

    public bool inRoachHunt;

    private float eatingTimerMax = 5;

   

    public bool eatingRoach = false;
 
    public bool inChase  = false;

    public float damageDistance = 4;
    public float sicknessDrainDistance = 10;
    private bool attackCoolingDown = false;

    public ActionTable actionTable;
   

    
    void Start()
    {
        //on start find compenents and set first position to goto equal to the beasts position
        agent = GetComponent<NavMeshAgent>();
        
        beastVision = GetComponent<BeastVision>();
        BeastCloseVision = GetComponent<BeastCloseVision>();
        player = GameObject.Find("Player");
       
        agent.SetDestination(centrePoint.position);

        actionTable = transform.GetComponent<ActionTable>();
        
    }
    //todo
    //Make gizmos for current state display above character
    
    void Update()
    {
        //caclulate distance to player
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;
        
        //if player is close enough to attack
        if(distanceToPlayer < damageDistance){
            //attack
            if(!attackCoolingDown){
            player.GetComponent<ActionManager>().health += -25;
            StartCoroutine(attackCoolDown());
            }
        }
        //if player is close enough to drain sickness and increase craving
        if(distanceToPlayer < sicknessDrainDistance){
            if(player.GetComponent<ActionManager>().sickness > 5){

            player.GetComponent<ActionManager>().sickness += -10*Time.deltaTime;
            }
            if(player.GetComponent<ActionManager>().craving< 99){
            player.GetComponent<ActionManager>().craving += +10*Time.deltaTime;
            }
        }


        //when player is vaping increase beast vision distance
        if(player.GetComponent<ActionManager>().vapeSound.isPlaying == true){
            beastVision.distance = 30f;
        }else{
            beastVision.distance = 5.8f;
        }

        

        


        //count the number of roaches in vision sphere and add them to a list
        //when we have found a roach, choose the first roach
       //assign previous value before assinging new
        roachInSightRangePrevious = roachInSightRange;
        //check if roach is in sight range
        int count = beastVision.Filter(roaches, "roaches");
        if(count > 0){
             roach = roaches[0];
             roachInSightRange = beastVision.IsInSight(roach);
             //fixes ordering issues when ...
             if(inChase){
                roachInSightRangePrevious = false;
             }
             
        }
        //start roach hunt          //im not sure why the previous != current thing but it breaks if i take it out
        if(roachInSightRange  && roachInSightRangePrevious != roachInSightRange && !inRoachHunt && !eatingRoach){
            inRoachHunt = true;
            huntedRoach = roach;
            debugStateColor = Color.yellow;
            beastIntChase.Stop();
            beastChase.Stop();
            beastRun.Stop();
           
            
            inChase = false;
            
            //play roach sound effect
            //play roach hunt animations
            //if reach raoach begin consumption
            // set chase timer and chase to false maybe?
            //make combo bool for priority actions
        }
        if(inRoachHunt && !eatingRoach && huntedRoach){
        HuntRoach();
        CheckIfOnRoach();
    }

   
        //golden roach insight, need list of golden roaches? and check with each one?
        //checxk if roach in vision isntead
        //always eats raoches priotizes golde nraoch take time to eat roaches 
        //maybe add reset audio functio nto call when changing states

        //assign previous value before assinging new
        playerInSightRangePrevious = playerInSightRange;
        //check if player is in sight range
        playerInSightRange = beastVision.IsInSight(player) || BeastCloseVision.IsInSight(player);
        if(!eatingRoach){
             playerInSightRangePrevious = false;
        }
        
       
            //when chase starts play int chase and play chase
       //if player is in sight range and wasnt before and not in chase, start chase
        if(playerInSightRange && (playerInSightRange != playerInSightRangePrevious) && !inChase && !inRoachHunt && !eatingRoach ){
            beastIntChase.pitch = UnityEngine.Random.Range(.9f,1.1f);
            beastIntChase.Play();
            beastChase.Play();
            beastRun.Play();
            beastWalk.Stop();
            beastBreathing.Stop();
            debugStateColor = Color.red;
            inChase = true;
        }



        //if your in sight range, chase, and cancel chase end timer
        if(inChase && !inRoachHunt && !eatingRoach){
            Chase();
            
        }
        //if player is in the sight range reset the chase timer
        if(playerInSightRange && inChase){
            chaseTimer = chaseTimerMax;
            chaseEndTimerActive = false;
        }

         if(!playerInSightRange && inChase){
            chaseEndTimerActive = true;
        }



     
        ChaseCountDown();
    
        
       
        
        //standard behaviour(factor into function?)
        if(agent.remainingDistance <= agent.stoppingDistance && !inChase && !idleing && !inRoachHunt &&!eatingRoach) //done with path And Not Chasing do normal action
        {
            
            //done with path reset animation
            beastAnim.SetBool("IsRunning?",false);
            beastAnim.SetBool("IsWalking",false);
            if(!beastBreathing.isPlaying){
            beastBreathing.Play();
            }

            
            actionTable.chooseAction();
               
               
               
        }
        
    
    }


    //sets variables for chase sequence and sets target to players position
     public void randomWalk(){
    debugStateColor = Color.green;
                    //look for point
                    bool foundPoint = false;
                    //while a point has not been found look for points
                    Vector3 point = centrePoint.position;
                    
                    while(!foundPoint){
                        foundPoint = RandomPoint(centrePoint.position, range, out point);

                    }



                        beastWalk.Play();
                        beastAnim.SetBool("IsWalking",true);
                        agent.SetDestination(point);
                        agent.stoppingDistance = 0;
                        agent.speed = walkSpeed;




     }
     
     public void approachPlayer(){
                debugStateColor = Color.magenta;
                beastAnim.SetBool("IsRunning?",true);
                agent.speed = 10f;
                agent.stoppingDistance = 65;
                agent.SetDestination(player.transform.position);
                


     }
     
     public void Idle(){

        
        StartCoroutine(IdleRoutine());
     }
      IEnumerator IdleRoutine(){
        idleing = true;
        beastAnim.SetBool("IsRunning?",false);
        beastAnim.SetBool("IsWalking",false);
        agent.SetDestination(centrePoint.position);
        yield return new WaitForSeconds(maxIdleTime);
        idleing = false;


     }
     private void Chase(){
       debugStateColor = Color.blue;
        beastWalk.Stop();
        beastAnim.SetBool("IsRunning?",true);
        beastAnim.SetBool("IsWalking",false);
                agent.speed = chaseSpeed;
                agent.stoppingDistance = 0;
                agent.SetDestination(player.transform.position);
       
                



    }
    //checks if player is not in sight range and just was, if so, it starts the chase end timer

    IEnumerator attackCoolDown(){
        attackCoolingDown = true;
        yield return new WaitForSeconds(3);
        attackCoolingDown = false;


    }
    //countdown for chase timer
    private void ChaseCountDown(){

         if(chaseTimer >= 0 && chaseEndTimerActive){
            
            
           
            chaseTimer = chaseTimer - Time.deltaTime;
        }

        //chase end
        if(chaseTimer <= .5 && chaseEndTimerActive){
           
            chaseEndTimerActive = false;
            inChase = false;
            agent.SetDestination(centrePoint.position);

            beastAnim.SetBool("IsRunning?",false);
            chaseActive = false;
            beastChase.Stop();
            beastBreathing.Play();
            beastRun.Stop();
            beastWalk.Play();
        }


    }
    //constant countdown for idle timer
   

    private void HuntRoach(){
         agent.speed = chaseSpeed;
                agent.stoppingDistance = 0.6f;
                agent.SetDestination(huntedRoach.transform.position);

    }
    IEnumerator EatRoach(){
        eatingRoach = true;
        yield return new WaitForSeconds(eatingTimerMax);
        eatingRoach = false;
        Destroy(huntedRoach);
        inRoachHunt = false;
        agent.SetDestination(centrePoint.position);
    }

    private void CheckIfOnRoach(){
        //agent.remainingDistance <= agent.stoppingDistance
        if(agent.remainingDistance <= agent.stoppingDistance ){
            //roach end
           
           
           StartCoroutine(EatRoach());
            

            //destroying causes lots of issues, maybe move roach to a different area, or convert its mesh to goop and untick a findable box.
            
          
            
            //timer to eat and nothing else
         
        }


    }

   
    //Random Walk function
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
   

    private void OnDrawGizmos(){
        Gizmos.color = debugStateColor;
    Gizmos.DrawSphere(new Vector3(transform.position.x,transform.position.y + 6,transform.position.z), 0.3f);


    }




    
}



