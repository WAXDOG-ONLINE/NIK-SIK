using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour {
    public List<GameObject> Stock = new List<GameObject>();
    public GameObject itemForSale;
    public GameObject instantiatePoint;

    public GameObject VendingParent;
    public Animator vendingAnimator;
    public AudioSource vendingSound;
    // Start is called before the first frame update

    public void Start() {
        foreach (Transform child in transform.parent) {
            if (child.GetComponent<Cash>()) {
                Stock.Add(child.gameObject);
            }
        }
    }
    public void sellItem() {
        if (Stock.Count > 0) {
            GameObject itemToRemove = Stock[0];

            Stock.RemoveAt(0);
            Destroy(itemToRemove);
            vendingAnimator.SetTrigger("Vend");
            vendingSound.Play();
            Instantiate(itemForSale, instantiatePoint.transform.position, Quaternion.identity);

        }



    }
}
