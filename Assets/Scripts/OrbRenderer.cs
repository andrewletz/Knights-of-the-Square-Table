using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRenderer : MonoBehaviour
{
    void Update()
    {
        // set y rotation to the same as the weapon sprite (facing the camera)
        Transform weaponSpriteTransform = transform.parent.transform.parent.GetChild(0).transform;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, weaponSpriteTransform.localEulerAngles.y, transform.localEulerAngles.z);   
    }
}
