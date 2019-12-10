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

    [Header("HeadSettings")]
    public Transform head;
    public float headRotationSpeed;
    public float headRotateAmount;

    [Header("Settings")]
    public LayerMask detectionMask;
    public float detectedTime;
    public float detectedDropTime;
    private float currentDetectLevel;
    public string detectionTag;
    public float positionDistance;

    [Header("Other")]
    public NavMeshAgent agent;
    public GameObject target;
    public Vector3 lastKnowPosition;
    private Vector3 startPos;

    public enum CurrentState { Idle, Following, Lost}
    public CurrentState currentState;

    public void Start()
    {
        startPos = transform.position;
    }

    public void Update()
    {
        UpdateDetection();
        switch (currentState)
        {
            case CurrentState.Following:
                SetWalkPosition();
                agent.SetDestination(lastKnowPosition);
                break;
            case CurrentState.Lost:
                if(Vector3.Distance(transform.position, lastKnowPosition) <= positionDistance)
                {
                    agent.SetDestination(startPos);
                    currentState = CurrentState.Idle;
                }
                break;
        }
    }

    public  IEnumerator HeadRotation()
    {
        bool right = true;
        float currentRotation = 0;
        while (currentState == CurrentState.Lost)
        {
            Vector3 rightDir = Quaternion.Euler(Vector3.up * headRotateAmount) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(Vector3.up * -headRotateAmount) * transform.forward;
            head.transform.LookAt(head.position + Vector3.Lerp(rightDir, leftDir, (currentRotation / 2f) + 0.5f));
            switch (right)
            {
                case true:
                    currentRotation += Time.deltaTime * headRotationSpeed;
                    if (currentRotation >= 1)
                        right = false;
                    break;
                case false:
                    currentRotation -= Time.deltaTime * headRotationSpeed;
                    if (currentRotation <= -1)
                        right = true;
                    break;
            }
            yield return null;
        }
    }

    public void SetWalkPosition()
    {
        if (target)
            lastKnowPosition = target.transform.position;
    }

    public void UpdateDetection()
    {
        if (target && currentState == CurrentState.Following)
            if (!CheckDetect().Contains(target))
            {
                currentState = CurrentState.Lost;
                StartCoroutine(HeadRotation());
                target = null;
            }

        if (!target)
            if (CheckDetect().Count > 0)
            {
                currentDetectLevel += Time.deltaTime;
                if (currentDetectLevel >= detectedTime || currentState == CurrentState.Lost)
                {
                    target = CheckDetect()[0];
                    currentState = CurrentState.Following;
                }
            }
            else if (currentDetectLevel >= 0 || currentState == CurrentState.Idle)
                currentDetectLevel -= detectedDropTime * Time.deltaTime;
    }

    public List<GameObject> CheckDetect()
    {
        List<GameObject> detectedTargets = new List<GameObject>();
        RaycastHit hit = new RaycastHit();
        foreach (Vector3 dir in GetDetectDirections())
            if (Physics.Raycast(head.position, dir, out hit, detectRange, detectionMask) && hit.transform.tag == detectionTag && !detectedTargets.Contains(hit.transform.gameObject))
            {
                detectedTargets.Add(hit.transform.gameObject);
                Debug.DrawLine(head.position, hit.point, Color.red);
            }

        return detectedTargets;
    }

    public Vector3[] GetDetectDirections()
    {
        int amount = Mathf.RoundToInt(detectWidth / detectDensity);

        Vector3 minDirX = Quaternion.Euler(Vector3.up * detectWidth) * head.forward;
        Vector3 maxDirX = Quaternion.Euler(-Vector3.up * detectWidth) * head.forward;

        Vector3 minDirY = Quaternion.Euler(head.right * detectWidth) * head.forward;
        Vector3 maxDirY = Quaternion.Euler(-head.right * detectWidth) * head.forward;

        List<Vector3> directions = new List<Vector3>();
        for (int x = 0; x < amount; x++)
            for (int y = 0; y < amount; y++)
                directions.Add((Vector3.Lerp(minDirX, maxDirX, 1f / amount * x) + Vector3.Lerp(minDirY, maxDirY, 1f / amount * y)).normalized);

        return directions.ToArray();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        if(head)
            foreach (Vector3 dir in GetDetectDirections())
                Gizmos.DrawRay(head.position, dir * detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(Vector3.up * headRotateAmount) * transform.forward);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(Vector3.up * -headRotateAmount) * transform.forward);
    }
}
