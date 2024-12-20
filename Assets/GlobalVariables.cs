using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    
    public  AudioClip dropSoundClip;
   
    public static GameObject player;

    public static int enemies = 0;

    public static int enemiesMax = 15;
    // Start is called before the first frame update
    void onAwake(){

player = GameObject.Find("Player");


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
