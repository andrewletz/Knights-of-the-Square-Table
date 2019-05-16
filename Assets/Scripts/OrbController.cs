﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    public GameObject FlamePillar;

    private GameObject orbRenderer;

    private Vector3 weaponSpritePosition;

    private float angle = 0f;
    private float height = 0.2f;
    private float radius = 0.5f;
    private float rotateSpeed = 1f;

    private bool travelling = false;

    void Start()
    {
        orbRenderer = transform.GetChild(0).transform.gameObject;
    }

    void Update()
    {
        // grabs the position of the floating weapon so we can rotate the orb around it
        weaponSpritePosition = transform.parent.GetChild(0).transform.position;

        if (!travelling) // if the orb isn't in movement, rotate it around the weapon
        {
            angle += rotateSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Cos(angle), height, Mathf.Sin(angle)) * radius;
            transform.position = Vector3.Lerp(transform.position, weaponSpritePosition + offset, rotateSpeed * Time.deltaTime);
        }
    }

    void CastSpell(Vector3 clickPos)
    {
        StartCoroutine(Spell(clickPos, 0.5f));
    }

    // travels to clickPos over travelTime (in seconds) and casts flame pillar
    IEnumerator Spell(Vector3 clickPos, float travelTime)
    {
        if (!travelling)
        {
            travelling = true;

            // travel towards the click position
            Vector3 startPosition = transform.position;
            float startTime = Time.time;
            float endTime = startTime + travelTime;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / travelTime;
                transform.position = Vector3.Slerp(startPosition, clickPos, progress);
                yield return null;
            }

            // spawn the spell
            yield return null;
            Instantiate(FlamePillar, clickPos, Quaternion.Euler(new Vector3(45, 0, 0)));

            // travel back to the weapon sprite
            startTime = Time.time;
            endTime = startTime + travelTime;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / travelTime;
                transform.position = Vector3.Slerp(clickPos, weaponSpritePosition, progress);
                yield return null;
            }

            travelling = false;
        }
    }
}
