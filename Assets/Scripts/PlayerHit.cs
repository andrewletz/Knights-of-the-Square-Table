using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("mortal")) {
    		other.GetComponent<EnemyController>().Death();
    		Debug.Log("Called");
    	}
    }
}
