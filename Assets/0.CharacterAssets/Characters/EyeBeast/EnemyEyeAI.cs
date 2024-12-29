using System.Collections;
using System.Collections.Generic;


using UnityEditor.Callbacks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.Animations; //important

//if you use this code you are contractually obligated to like the YT video
public class EnemyEyeAI : MonoBehaviour //don't forget to change the script name if you haven't
{

    //SOUND EFFECTS

    public SkinnedMeshRenderer eyeMeshBody;
    public MeshRenderer eyeMesh;
    private Color debugStateColor = Color.gray;


    [SerializeField]
    private AudioSource eyeIdle;
    [SerializeField]
    private AudioSource eyeAgro;



    //list of possible roaches 1 is the max

    //target Roach






    public GameObject player;

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area



    //Nav ADD SENSOR CIRCLE SMALL RADIUS AROUDN GUY
    public BeastVision beastVision;

    public NavMeshAgent agent;









    public bool playerInSightRange;


    //Chase params




    //random walk params
    public float walkSpeed = 2;
    public float range = 10; //radius of sphere



    //Idle params
    private bool idleing;
    private float idleTimer;
    public float maxIdleTime = 4;

    public float runSpeed = 30;

    //other








    void Start() {
        //on start find compenents and set first position to goto equal to the beasts position
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(centrePoint.position);
        beastVision = GetComponent<BeastVision>();
        player = GameObject.Find("Player");


    }
    //todo
    //Make gizmos for current state display above character

    void Update() {
        //when player is vaping increase beast vision distance
        Vector3 dir = player.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        if (idleing) {
            idleCountDown();
        }




        //assign previous value before assinging new

        //check if player is in sight range
        playerInSightRange = beastVision.IsInSight(player);
        if (playerInSightRange) {
            if (player.GetComponent<ActionManager>().sickness < 100) {
                player.GetComponent<ActionManager>().sickness = player.GetComponent<ActionManager>().sickness + Time.deltaTime * 4;
            }
            if (!eyeAgro.isPlaying) {

                eyeIdle.Stop();
                eyeAgro.Play();
            }


        }
        else {
            if (!eyeIdle.isPlaying) {
                eyeIdle.Play();
                eyeAgro.Stop();
            }

        }







        //standard behaviour(factor into function?)
        if (agent.remainingDistance <= agent.stoppingDistance && !idleing) //done with path And Not Chasing do normal action
        {

            //done with path reset animation






            //choose new point
            int choice = Random.Range(1, 11);

            if (choice <= -1) {
                debugStateColor = Color.green;
                //look for point
                bool foundPoint = false;
                //while a point has not been found look for points
                Vector3 point = centrePoint.position;

                while (!foundPoint) {
                    foundPoint = RandomPoint(centrePoint.position, range, out point);

                }




                agent.SetDestination(point);
                agent.stoppingDistance = 0;
                agent.speed = walkSpeed;





            }
            else if (choice >= 9 && choice <= 10) {
                debugStateColor = Color.blue;

                idleing = true;
                idleTimer = maxIdleTime;

                agent.SetDestination(centrePoint.position);



            }
            else if (choice >= 0 && choice <= 8) {
                debugStateColor = Color.magenta;

                agent.speed = runSpeed;
                agent.stoppingDistance = 10;
                agent.SetDestination(player.transform.position);






            }

        }


    }


    //sets variables for chase sequence and sets target to players position

    private void idleCountDown() {
        if (idleTimer >= 0 && idleing == true) {

            idleTimer = idleTimer - Time.deltaTime;

        }

        if (idleTimer <= 0.5 && idleing == true) {

            idleing = false;
        }


    }




    //Random Walk function
    bool RandomPoint(Vector3 center, float range, out Vector3 result) {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
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


    private void OnDrawGizmos() {
        Gizmos.color = debugStateColor;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 6, transform.position.z), 0.3f);


    }





}
