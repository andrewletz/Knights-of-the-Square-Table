using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public GameObject mainPlayer;
    public float height = 1.5f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start(){
        transform.position = new Vector3(mainPlayer.transform.position.x, height, mainPlayer.transform.position.z);
        offset = transform.position - mainPlayer.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = mainPlayer.transform.position + offset;
    }
}