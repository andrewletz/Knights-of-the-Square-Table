using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mainPlayer;
    public float playerDistance = 2.0f, height = 4.0f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start(){
        transform.position = new Vector3(mainPlayer.transform.position.x, height, mainPlayer.transform.position.z - playerDistance);
        offset = transform.position - mainPlayer.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = mainPlayer.transform.position + offset;
    }
}