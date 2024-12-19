using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Drink : MonoBehaviour
{

    public float liquidPercentage = 100;

    public Liquid liquid;
    public float maxFill;
    public float minFill;

    public float fillLevel;

    private float originalSellValue;
    // Start is called before the first frame update

    void Start(){

        originalSellValue = transform.GetComponent<ItemInfo>().sellValue;
    }
    void Update(){

        
    }


    public void drainLiquid(){
        if(liquidPercentage > 0){

        liquidPercentage = liquidPercentage - 10*Time.deltaTime;

         fillLevel = math.remap(0,100,minFill,maxFill,liquidPercentage);
        liquid.fillAmount = fillLevel;
        transform.GetComponent<ItemInfo>().sellValue =1 + (originalSellValue - 1)* (liquidPercentage/100);
        }
    }
}
