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

    //Uodate
    public void Update()
    {
        Cloak();
    }

    //Cloak
    ///Switches from cloak mode
    public void Cloak()
    {
        if (Input.GetButtonDown(cloakInput))
        {
            cloaked = !cloaked;
            cloak.SetActive(cloaked);
        }
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
