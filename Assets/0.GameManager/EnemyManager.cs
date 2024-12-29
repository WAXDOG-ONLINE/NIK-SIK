using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
public class EnemyManager : MonoBehaviour {

    public int eyeEnemyMax = 1;
    public bool doSpawnEyes = true;

    public GameObject player;
    [SerializeField]
    private GameObject eyePrefab;

    private Vector3 spawnPoint;

    public float spawnRangeMin = 0;
    public float spawnRangeMax = 20;


    List<EnemyEyeAI> eyeEnemies = new List<EnemyEyeAI>();
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (player.GetComponent<ActionManager>().craving > 50) {
            spawnEye();
        }

        if (player.GetComponent<ActionManager>().craving > 70) {
            eyeEnemyMax = 2;
        }

        if (player.GetComponent<ActionManager>().craving > 90) {
            eyeEnemyMax = 3;
        }



        if (player.GetComponent<ActionManager>().craving < 50) {
            despawnEyes();
        }
        updateEyeMaterial(player.GetComponent<ActionManager>().craving);

    }

    public void spawnEye() {
        if (doSpawnEyes) {
            if (eyeEnemies.Count < eyeEnemyMax) {

                if (findSpawnPoint()) {
                    GameObject newEye = Instantiate(eyePrefab, spawnPoint, Quaternion.identity);
                    eyeEnemies.Add(newEye.GetComponent<EnemyEyeAI>());
                }




            }






        }
    }

    public void despawnEyes() {
        foreach (EnemyEyeAI eye in eyeEnemies) {

            Destroy(eye.gameObject);

        }
        eyeEnemies = new List<EnemyEyeAI>();
        eyeEnemyMax = 1;




    }
    public void updateEyeMaterial(float craving) {
        foreach (EnemyEyeAI eye in eyeEnemies) {
            float eyeAlpha = math.remap(50, 100, 0, 1, craving);
            Color eyeColor = eye.eyeMesh.material.color;
            Color eyeBodyColor = eye.eyeMeshBody.material.color;
            eye.eyeMesh.material.color = new Color(eyeColor.r, eyeColor.g, eyeColor.b, eyeAlpha);
            eye.eyeMeshBody.material.color = new Color(eyeBodyColor.r, eyeBodyColor.g, eyeBodyColor.b, eyeAlpha);

        }



    }

    private bool findSpawnPoint() {

        Vector3 randomPoint = player.transform.position + UnityEngine.Random.insideUnitSphere * spawnRangeMax; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            if ((hit.position - player.transform.position).magnitude > spawnRangeMin) {

                spawnPoint = hit.position;
                return true;
            }
        }

        spawnPoint = Vector3.zero;
        return false;
    }

}
