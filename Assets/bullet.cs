using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bullet : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Destroy(transform.gameObject);
            other.gameObject.GetComponent<ActionManager>().takeDamage(1);
        }
        StartCoroutine(DestroyBullet());



    }

    IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(5f);
        if (transform.gameObject != null) {
            Destroy(transform.gameObject);
        }

    }
}
