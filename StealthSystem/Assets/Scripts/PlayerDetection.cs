using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Checks")]
    public bool cloaked;

    [Header("Cloak Settings")]
    public GameObject cloak;
    public string cloakInput;
    public float cloakTime;
    private float currentCloakTime;

    public void Start()
    {
        currentCloakTime = cloakTime;
    }

    //Uodate
    public void Update()
    {
        Cloak();
    }

    //Cloak
    ///Switches from cloak mode
    public void Cloak()
    {
        if (Input.GetButtonDown(cloakInput) && currentCloakTime > 0)
        {
            cloaked = !cloaked;
            cloak.SetActive(cloaked);
        }

        if (cloaked)
        {
            currentCloakTime -= Time.deltaTime;
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical") || currentCloakTime <= 0)
            {
                cloaked = false;
                cloak.SetActive(cloaked);
            }
        }
        else if (currentCloakTime < cloakTime)
            currentCloakTime += Time.deltaTime;
    }

    //Can Be detected
    ///This is called to check if the player is detectable
    public bool CanBeDetected()
    {
        bool detected = true;

        if (cloaked)
            detected = false;

        return detected;
    }
}
