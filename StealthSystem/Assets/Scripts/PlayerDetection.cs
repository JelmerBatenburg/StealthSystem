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

    public void Update()
    {
        Cloak();
    }

    public void Cloak()
    {
        if (Input.GetButtonDown(cloakInput))
        {
            cloaked = !cloaked;
            cloak.SetActive(cloaked);
        }
    }

    public bool CanBeDetected()
    {
        bool detected = true;

        if (cloaked)
            detected = false;

        return detected;
    }
}
