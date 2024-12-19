using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    
    public  AudioClip dropSoundClip;
    [SerializeField]
    public static GameObject player;
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
