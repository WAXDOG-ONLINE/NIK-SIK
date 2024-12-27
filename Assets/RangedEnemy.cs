using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RangedEnemy : MonoBehaviour
{

    public BeastVision beastVision;
    public bool playerInSightRange;

    public GameObject player;

    public GameObject bullet;

    private bool inCooldown;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        beastVision = GetComponent<BeastVision>();
       
        
    }

    // Update is called once per frame
    void Update()
    {
         playerInSightRange = beastVision.IsInSight(player);
         
        if(playerInSightRange && !inCooldown){
            StartCoroutine(CoolDown()); //Start Cooldown
            StartCoroutine(shootBullet()); //Shoot Bullet
            
        }
        
    }

    IEnumerator shootBullet(){
        yield return new WaitForSeconds(1f);
        GameObject bulletInstance = Instantiate(bullet, new Vector3(transform.position.x,transform.position.y +1, transform.position.z), UnityEngine.Quaternion.identity);
        Vector3 bulletDirection = player.transform.position - transform.position;
        Vector3 bulletVelocity =  25*(bulletDirection.normalized);
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletVelocity;



        
    }

    IEnumerator CoolDown(){
        inCooldown = true;
        yield return new WaitForSeconds(3f);
        inCooldown = false;
    }
}
