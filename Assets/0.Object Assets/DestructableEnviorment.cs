using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructableEnviorment : MonoBehaviour {

    [SerializeField]
    private GameObject DestroyedPrefab;
    [SerializeField]
    private GameObject OriginalPrefab;
    [SerializeField]
    private AudioSource destructionSound;
    [SerializeField]
    private float pitchMin = 1;
    [SerializeField]
    private float pitchMax = 1.7f;
    [SerializeField]
    private float despawnTime = 10;

    private bool blownUp = false;

    // Start is called before the first frame update


    public void Blowup() {

        OriginalPrefab.SetActive(false);
        DestroyedPrefab.SetActive(true);
        StartCoroutine(Despawn());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {


            if (blownUp == false) {

                Blowup();
                destructionSound.pitch = Random.Range(pitchMin, pitchMax);
                destructionSound.Play();
                blownUp = true;
                if (transform.GetComponent<BoxCollider>()) {
                    transform.GetComponent<BoxCollider>().enabled = false;
                }
            }

        }



    }
    IEnumerator Despawn() {


        yield return new WaitForSeconds(despawnTime);
        Destroy(transform.gameObject);
    }
}
