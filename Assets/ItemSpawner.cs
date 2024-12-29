using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public List<GameObject> lootTable = new List<GameObject>();
    // Start is called before the first frame update
    void Start() {
        int itemChoice = UnityEngine.Random.Range(0, lootTable.Count);
        GameObject chosenItem = lootTable[itemChoice];
        // float yOffset = chosenItem.GetComponent<BoxCollider>().size.y/2;
        float yOffset = 0;
        Instantiate(chosenItem, new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), quaternion.identity);


    }

    // Update is called once per frame
    void Update() {

    }
}
