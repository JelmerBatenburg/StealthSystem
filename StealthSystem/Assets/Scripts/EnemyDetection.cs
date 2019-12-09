using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("DetectionArea")]
    public float detectWidth;
    public float detectDensity;
    public float detectRange;

    [Header("Settings")]
    public LayerMask detectionMask;
    public float detectedTime;
    public float detectedDropTime;
    private float currentDetectLevel;
    public string detectionTag;

    public GameObject target;

    public void Update()
    {
        UpdateDetection();
    }

    public void UpdateDetection()
    {
        if (target)
            if (!CheckDetect().Contains(target))
                target = null;

        if (!target)
            if (CheckDetect().Count > 0)
            {
                currentDetectLevel += Time.deltaTime;
                if (currentDetectLevel >= detectedTime)
                    target = CheckDetect()[0];
            }
            else if (currentDetectLevel >= 0)
                currentDetectLevel -= detectedDropTime * Time.deltaTime;
    }

    public List<GameObject> CheckDetect()
    {
        List<GameObject> detectedTargets = new List<GameObject>();
        RaycastHit hit = new RaycastHit();
        foreach (Vector3 dir in GetDetectDirections())
            if (Physics.Raycast(transform.position, dir, out hit, detectRange, detectionMask) &&  hit.transform.tag == detectionTag)
                detectedTargets.Add(hit.transform.gameObject);

        return detectedTargets;
    }

    public Vector3[] GetDetectDirections()
    {
        int amount = Mathf.RoundToInt(detectWidth / detectDensity);

        Vector3 minDirX = Quaternion.Euler(Vector3.up * detectWidth) * transform.forward;
        Vector3 maxDirX = Quaternion.Euler(-Vector3.up * detectWidth) * transform.forward;

        Vector3 minDirY = Quaternion.Euler(Vector3.right * detectWidth) * transform.forward;
        Vector3 maxDirY = Quaternion.Euler(-Vector3.right * detectWidth) * transform.forward;

        List<Vector3> directions = new List<Vector3>();
        for (int x = 0; x < amount; x++)
            for (int y = 0; y < amount; y++)
                directions.Add((Vector3.Lerp(minDirX, maxDirX, 1f / amount * x) + Vector3.Lerp(minDirY, maxDirY, 1f / amount * y)).normalized);

        return directions.ToArray();
    }

    public void OnDrawGizmos()
    {
        foreach (Vector3 dir in GetDetectDirections())
            Gizmos.DrawRay(transform.position, dir * detectRange);
    }
}
