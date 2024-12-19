using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cig : MonoBehaviour
{

    public bool isSmoked = false;

    public ActionManager actionManager;


    private void Start(){
        

        actionManager = GameObject.Find("Player").GetComponent<ActionManager>();

    }

   
}
