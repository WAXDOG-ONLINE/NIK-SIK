using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    [SerializeField]
    private float explosionForce = 4;
    private bool isDestroyed = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(isDestroyed == false){
            isDestroyed = true;
        
        foreach(Transform child in transform){
            float rx = Random.Range(-1*explosionForce,explosionForce);
            float ry =Random.Range(0,explosionForce);
            float rz =Random.Range(-1*explosionForce,explosionForce);
            child.GetComponent<Rigidbody>().velocity = new Vector3(rx,ry,rz);
        }}
    }
}
