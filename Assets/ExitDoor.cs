using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ExitDoor : MonoBehaviour {
    public bool isOpen = false;
    public List<Lock> locks = new List<Lock>();

    public Animator doorAnim;

    // Start is called before the first frame update


    // Update is called once per frame
    void Update() {
        int validLocks = 0;
        foreach (Lock locky in locks) {
            if (locky.hasKey == true) {
                validLocks = validLocks + 1;

            }
        }
        if (validLocks >= locks.Count) {

            doorAnim.SetBool("Open", true);
        }

    }
}
