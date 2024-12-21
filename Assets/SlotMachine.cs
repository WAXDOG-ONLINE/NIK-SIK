using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public GameObject wheel1;
    public GameObject wheel2;
    public GameObject wheel3;

    int spinIndex1 = 0;
    int spinIndex2 =0;
    int spinIndex3 = 0;

    int greatestSpin = 0;

    bool playing = false;

    public float slotPrice = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spin(Transform player){
    
    //rotate respective wheels over time by 22.5 degrees for there respective spins
    
    if(playing == false && player.GetComponent<ActionManager>().cashBalace > slotPrice){
        player.GetComponent<ActionManager>().cashBalace -= slotPrice;
    StartCoroutine(playGame());
    }
    }
    
    IEnumerator playGame(){
        playing = true;
            int spin1 = Random.Range(10, 20);
            int spin2 = Random.Range(10, 10);
             int spin3 = Random.Range(10, 29);
             spinIndex1 = (spinIndex1 + spin1) % 8;
            spinIndex2 = (spinIndex2 + spin2) % 8;
             spinIndex3 = (spinIndex3 + spin3) % 8;
             int greatestSpin = Mathf.Max(spin1, spin2, spin3);
        StartCoroutine(RotateWheel(wheel1, spin1 * 45, .15f*spin1));
        StartCoroutine(RotateWheel(wheel2, spin2 * 45, .15f*spin2));
        StartCoroutine(RotateWheel(wheel3, spin3 * 45, .15f*spin3));
        yield return new WaitForSeconds(greatestSpin * .15f);
        Debug.Log("Over");
        if(spinIndex1 == spinIndex2 && spinIndex2 == spinIndex3){
            Debug.Log("You Win!");
            
    }
    playing = false;
  
    }
    IEnumerator RotateWheel(GameObject wheel, float angle, float duration){
        float startRotation = wheel.transform.eulerAngles.z;
        float endRotation = startRotation + angle;
        float t = 0.0f;
        while(t < duration){
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, zRotation);
            yield return null;
        }
        wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, endRotation);
    }
    
}
