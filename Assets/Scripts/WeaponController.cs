using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed = 8f;

    private float angle = 0f;
    private Vector3 playerPosition;

    private GameObject weaponRenderer;

    void Start()
    {
        weaponRenderer = transform.GetChild(0).transform.gameObject;
    }

    void Update()
    {
        // set rotation to mouse location
        Vector3 temp = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(temp), Time.deltaTime * rotateSpeed);

        // set rotation of sprite renderer's y to opposite of this object's rotation y
        weaponRenderer.SendMessage("SetYAngle", -transform.localEulerAngles.y);
    }

    // called by player controller to pass mouse angle
    void GetAngle(float newAngle) 
    {
        this.angle = newAngle;
    }

    //void StartAttackAnimation()
    //{
    //    float angleSword = Mathf.Abs(angle - 90) + 45;
    //    weaponRenderer.SendMessage("AttackAnimation", angleSword);
    //}

    // public float radius = 0.6f;
    // public float height = 0.2f;
    /* Previous method of rotation - left for reference
    void FixedUpdate()
    {
        playerPosition = transform.parent.position;
        Vector3 offset = new Vector3(Mathf.Cos(angle), height, Mathf.Sin(angle)) * radius;
        transform.position = playerPosition + offset;

        // if you want LERPed movement (causes issues)
        //transform.position = Vector3.Lerp(transform.position, playerPosition + offset, rotateSpeed * Time.deltaTime);
    } 
    */
}
