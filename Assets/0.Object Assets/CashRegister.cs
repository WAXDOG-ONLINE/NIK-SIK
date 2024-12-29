using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashRegister : MonoBehaviour {
    public ActionManager player;
    public TextMeshPro balance;

    public AudioSource chaChing;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        player = GameObject.Find("Player").GetComponent<ActionManager>();
        if (player.cashBalace < 10) {
            balance.text = "00" + player.cashBalace + "$";
        }
        if (player.cashBalace > 9 && player.cashBalace < 99) {
            balance.text = "0" + player.cashBalace + "$";
        }
        if (player.cashBalace > 99) {
            balance.text = player.cashBalace + "$";
        }

    }
}
