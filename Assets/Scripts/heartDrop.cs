using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heartDrop : MonoBehaviour
{

    public int healAmount = 10;

	private GameObject playerObject;

    void Start()
    {
        playerObject = GameObject.Find("Player");
    }
	
    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("Player")) {
    		playerObject.GetComponent<PlayerController>().Heal(healAmount);
            Destroy(this.gameObject);
    	}
    }
}