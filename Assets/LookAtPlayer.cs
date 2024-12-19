using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;
  
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
         Vector3 dir = player.transform.position - transform.position;
         Quaternion lookRotation = Quaternion.LookRotation(dir);
        
         transform.rotation = Quaternion.Euler(transform.eulerAngles.x, lookRotation.eulerAngles.y, transform.eulerAngles.z);
        
    }
}
