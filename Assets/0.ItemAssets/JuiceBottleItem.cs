using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceBottle : MonoBehaviour {
    public Liquid liquid;
    public float maxFill;
    public float minFill;

    public float juicePercentage;

    public float juiceDrainRate;

    public float fillLevel;

    public float shaderShapeAdjust;





    public string compatibility;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //map fill level from juicepercentage
        fillLevel = minFill + ((maxFill - minFill) / (100 - 0)) * (juicePercentage - 0);



        liquid.fillAmount = fillLevel;
    }

    public void JuiceDrain() {
        if (juicePercentage > 0f) {
            juicePercentage = juicePercentage - 0.0025f * 5;

        }
    }
}
