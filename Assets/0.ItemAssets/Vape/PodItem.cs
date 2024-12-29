using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodItem : MonoBehaviour {
    public bool isFillable = false;
    public Liquid liquid;
    public float maxFill;
    public float minFill;

    public float juicePercentage;

    public float juiceDrainRate;

    public float fillLevel;

    public float shaderShapeAdjust;

    public Vector3 podPositionOffset;



    public string compatibility;
    // Start is called before the first frame update
    void Start() {
        //editing base material in order to create a new instance of it
        liquid.gameObject.GetComponent<MeshRenderer>().material = liquid.gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        //map fill level from juicepercentage

    }

    public void JuiceDrain() {
        if (juicePercentage > 0f) {
            juicePercentage = juicePercentage - .0025f * juiceDrainRate;

            fillLevel = minFill + ((maxFill - minFill) / (100 - 0)) * (juicePercentage - 0);



            liquid.fillAmount = fillLevel;
        }
    }

    public void JuiceFill() {

        juicePercentage = juicePercentage + 0.0025f * 10;

        fillLevel = minFill + ((maxFill - minFill) / (100 - 0)) * (juicePercentage - 0);



        liquid.fillAmount = fillLevel;




    }
}
