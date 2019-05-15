using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRenderer : MonoBehaviour
{

    private bool animating = false;
    private int swipeRange = 120;

    void SetYAngle(float angle) 
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
    }

    void SetZAngle(float angle) 
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
    }

    void AttackAnimation(float angle)
    {
        //Quaternion from = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, angle + 90);
        //Quaternion to = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, angle - 90);

        Vector3 from = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle + swipeRange);
        Vector3 to = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle - swipeRange);
        StartCoroutine(RotateOverTime(from, to, transform.localEulerAngles, 0.35f));
    }

    IEnumerator RotateOverTime(Vector3 startRotation, Vector3 finalRotation, Vector3 returnRotation, float duration)
    {
        if (!animating)
        {
            animating = true;
            if (duration > 0f)
            {
                float startTime = Time.time;
                float endTime = startTime + duration;
                transform.localEulerAngles = startRotation;
                yield return null;
                while (Time.time < endTime)
                {
                    float progress = (Time.time - startTime) / duration;
                    // progress will equal 0 at startTime, 1 at endTime.
                    transform.localEulerAngles = Vector3.Lerp(startRotation, finalRotation, progress);
                    yield return null;
                }
            }
            transform.localEulerAngles = returnRotation;
            animating = false;
        }
    }
}
