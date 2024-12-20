using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionTable : MonoBehaviour
{
    [Header("drops")]
    [SerializeField]
    private ActionEvent[] ActionEvents;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    

    public void chooseAction(){
        float totalChance = 0;
        foreach(ActionEvent action in ActionEvents){

            totalChance += action.actionChance;
        }


        float rand = Random.Range(0f,totalChance);
        float cumulativeChance = 0f;


        foreach(ActionEvent action in ActionEvents){

            cumulativeChance += action.actionChance;


            if(rand <= cumulativeChance){
           
            action.action.Invoke();
            
            
            
            //allows for call of functions assigned in editor
            //lootItem.GenLoot.Invoke();
            return;
        }



        }
        
        


    }
}

[System.Serializable]
public class ActionEvent
{

[Space]
[Space]
public string actionName;
[Range(0f,1f)] public float actionChance = 0.5f;

public UnityEvent action;

//allows for assignment of functions in inspector




}
