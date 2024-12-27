using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LaserRangeEnemy : MonoBehaviour
{

    public BeastVision beastVision;
    public bool playerInSightRange;

    public GameObject player;

    public float timeToShoot = 3f;

    private float timeToShootTimer = 0f;

    public AudioSource laserSound;

    public LineRenderer laserRenderer;

    public GameObject laserSource;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        beastVision = GetComponent<BeastVision>();
        timeToShootTimer = timeToShoot;
       
        
    }

    // Update is called once per frame
    void Update()
    {
         playerInSightRange = beastVision.IsInSight(player);

         //check if player is in sight range for long if enough
         //if it is then start cooldown and shoot bullet

         
        if(playerInSightRange){
            timeToShootTimer -= Time.deltaTime;


        
            
        }else{
            timeToShootTimer = timeToShoot;
        }
        if(timeToShootTimer <= 0 ){
           //ShootPlayer
            player.GetComponent<ActionManager>().takeDamage(1);
           timeToShootTimer = timeToShoot;
           laserSound.Play();
           laserRenderer.positionCount = 2;
           laserRenderer.SetPosition(0, laserSource.transform.position);
           
            laserRenderer.SetPosition(1, player.transform.position);

            
        }
        
    }}

