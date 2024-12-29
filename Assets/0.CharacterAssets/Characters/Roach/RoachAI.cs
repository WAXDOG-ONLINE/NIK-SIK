using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class RoachAI : MonoBehaviour {
    public NavMeshAgent agent;

    public LayerMask whatIsGround;


    void Awake() {

        agent = GetComponent<NavMeshAgent>();

    }

    void Update() {
        Patroling();
    }
    //patrolling
    public UnityEngine.Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Start is called before the first frame update



    private void Patroling() {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        UnityEngine.Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint() {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new UnityEngine.Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    // Update is called once per frame

}
