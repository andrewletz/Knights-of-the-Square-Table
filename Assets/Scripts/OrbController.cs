using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbController : MonoBehaviour
{
    public GameObject FlamePillar;
    public GameObject Implosion;

    private GameObject currentSpell;
    private int spellNumber; // 0 flame pillar, 1 implosion
    private int flamePillarCd = 4;
    private bool canUseFlamePillar = true;

    private int implosionCd = 6;
    private bool canUseImplosion = true;

    private GameObject UIflamePillarCd;
    private GameObject UIimplosionCd;

    private GameObject orbRenderer;

    private Vector3 weaponSpritePosition;

    private float angle = 0f;
    private float height = 0.2f;
    private float radius = 0.5f;
    private float rotateSpeed = 1f;

    private bool travelling = false;

    void Start()
    {
        currentSpell = FlamePillar;
        spellNumber = 0;
        orbRenderer = transform.GetChild(0).transform.gameObject;

        UIflamePillarCd = GameObject.Find("FireSpellBackground").transform.GetChild(1).gameObject;
        UIimplosionCd = GameObject.Find("MagnetSpellBackground").transform.GetChild(1).gameObject;
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

    void SetSpell(string spellName)
    {
        if (spellName == "FlamePillar")
        {
            currentSpell = FlamePillar;
            spellNumber = 0;
        }
        else
        {
            currentSpell = Implosion;
            spellNumber = 1;
        }
    }

    void CastSpell(Vector3 clickPos)
    {
        if (travelling) return;
        if (spellNumber == 0 && canUseFlamePillar)
        {
            StartCoroutine(Spell(clickPos, 0.5f));
            StartCoroutine(FlamePillarCooldown());
        } else if (spellNumber == 1 && canUseImplosion)
        {
            StartCoroutine(Spell(clickPos, 0.5f));
            StartCoroutine(ImplosionCooldown());
        }
    }

    IEnumerator FlamePillarCooldown()
    {
        UIflamePillarCd.SetActive(true);
        canUseFlamePillar = false;
        int cd = flamePillarCd;
        while (cd > 0)
        {
            cd -= 1;
            UIflamePillarCd.GetComponentInChildren<Text>().text = "" + (cd + 1);
            yield return new WaitForSeconds(1);
        }
        canUseFlamePillar = true;
        UIflamePillarCd.SetActive(false);
    }

    IEnumerator ImplosionCooldown()
    {
        UIimplosionCd.SetActive(true);
        canUseImplosion = false;
        int cd = implosionCd;
        while (cd > 0)
        {
            cd -= 1;
            UIimplosionCd.GetComponentInChildren<Text>().text = "" + (cd + 1);
            yield return new WaitForSeconds(1);
        }
        canUseImplosion = true;
        UIimplosionCd.SetActive(false);
    }

    // travels to clickPos over travelTime (in seconds) and casts currentSpell
    IEnumerator Spell(Vector3 clickPos, float travelTime)
    {
        Vector3 startPosition = transform.position;
        GameObject spellToCast = currentSpell;
        RaycastHit hit;

        clickPos = new Vector3(clickPos.x, -4.5f, clickPos.z);
        
        bool hitWall = false;
        if (Physics.Linecast(startPosition, clickPos, out hit) && hit.transform.tag == "Wall"){
            hitWall = true;
        }
        
        if (!travelling && !hitWall)
        {
            travelling = true;

            // travel towards the click position
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
            Instantiate(spellToCast, clickPos, Quaternion.Euler(new Vector3(45, 0, 0)));

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
