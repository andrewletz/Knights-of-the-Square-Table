using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
	private GameObject gameManagerObject;
    
    //test
    public AudioClip portalEnter;

    void Start()
    {
        gameManagerObject = GameObject.Find("GameManager");
    }
	
    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("Player")) {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(portalEnter);
            gameManagerObject.GetComponent<GameManager>().NextLevel();
            Destroy(this.gameObject);
    	}
    }
}
