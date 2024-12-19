using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LootTable : MonoBehaviour
{
    [Header("drops")]
    [SerializeField]
    private Loot[] lootItems;
    // Start is called before the first frame update
    void Start()
    {
        chooseItem();
    }

    // Update is called once per frame
    

    private void chooseItem(){
        float totalChance = 0;
        foreach(Loot lootItem in lootItems){

            totalChance += lootItem.DropChance;
        }


        float rand = Random.Range(0f,totalChance);
        float cumulativeChance = 0f;


        foreach(Loot lootItem in lootItems){
            cumulativeChance += lootItem.DropChance;


            if(rand <= cumulativeChance){
           
            GameObject itemSpawned = Instantiate(lootItem.itemPrefab,transform.position,Quaternion.identity);
            itemSpawned.transform.parent = transform;
            
            
            //allows for call of functions assigned in editor
            //lootItem.GenLoot.Invoke();
            return;
        }



        }
        
        


    }
}

[System.Serializable]
public class Loot
{

[Space]
[Space]
public GameObject itemPrefab;
[Range(0f,1f)] public float DropChance = 0.5f;



//allows for assignment of functions in inspector


//public UnityEvent GenLoot;


}
