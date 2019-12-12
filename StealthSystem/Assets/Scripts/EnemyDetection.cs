using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDetection : MonoBehaviour
{
    [Header("DetectionArea")]
    public float detectWidth;
    public float detectDensity;
    public float detectRange;

    [Header("DestectSettings")]
    public Transform detectPosition;

    [Header("Settings")]
    public LayerMask detectionMask;
    public float detectedTime;
    public float detectedDropTime;
    [HideInInspector]
    public float currentDetectLevel;
    public string detectionTag;

    public List<GameObject> CheckDetect()
    {
        Collider[] detectedColliders = Physics.OverlapSphere(detectPosition.position, detectRange, detectionMask);
        List<GameObject> detectedPlayers = new List<GameObject>();

        foreach (Collider col in detectedColliders)
        {
            Vector3 dir = col.transform.position - detectPosition.position;
            RaycastHit hit = new RaycastHit();
            if (Mathf.Abs(Vector3.Angle(dir, detectPosition.forward)) <= detectWidth)
                if (Physics.Raycast(detectPosition.position, dir, out hit) && hit.collider == col)
                {
                    detectedPlayers.Add(col.gameObject);
                    Debug.DrawLine(detectPosition.position, col.transform.position, Color.red);
                }
        }

        return detectedPlayers;
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawRay(detectPosition.position, (Quaternion.Euler(detectPosition.up * detectWidth) * detectPosition.forward) * detectRange);
        Gizmos.DrawRay(detectPosition.position, (Quaternion.Euler(-detectPosition.up * detectWidth) * detectPosition.forward) * detectRange);
        Gizmos.color = Color.red;
    }
}
