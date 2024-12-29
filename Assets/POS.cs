using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class POS : MonoBehaviour {
    public float price = 0;
    public bool sold = false;
    public TextMeshPro priceText;
    public AudioSource chaChing;

    public GameObject displayCase;
    // Start is called before the first frame update
    void Start() {
        priceText.text = price + "$";

    }

    // Update is called once per frame
    public void Sell() {
        Destroy(displayCase);
        sold = true;
        priceText.text = "$old";
        //play CHA-CHING
        //add speaker texture onto POS mesh
        chaChing.pitch = Random.Range(0.9f, 1.1f);
        chaChing.Play();

    }
}
