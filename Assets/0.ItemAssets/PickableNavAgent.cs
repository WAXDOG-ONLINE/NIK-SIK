using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//resets nav mesh agents who ahve been picked up once they hit the ground
public class PickableNavAgent : MonoBehaviour {

    private bool agentIsOnMesh;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update() {
        agentIsOnMesh = IsAgentOnNavMesh(transform.gameObject);

        if (agentIsOnMesh == true) {
            transform.GetComponent<NavMeshAgent>().enabled = true;
        }




    }





    public bool IsAgentOnNavMesh(GameObject agentObject) {
        Vector3 agentPosition = agentObject.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, .25f, NavMesh.AllAreas)) {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z)) {
                // Lastly, check if object is below navmesh
                return true;
            }
        }

        return false;
    }
}
