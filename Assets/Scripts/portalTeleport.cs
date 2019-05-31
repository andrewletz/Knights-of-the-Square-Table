using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalTeleport : MonoBehaviour
{
	private GameObject gameManagerObject;

    void Start()
    {
        gameManagerObject = GameObject.Find("GameManager");
    }
	
    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("Player")) {
    		gameManagerObject.GetComponent<GameManager>().NextLevel();
            Destroy(this.gameObject);
    	}
    }
}
