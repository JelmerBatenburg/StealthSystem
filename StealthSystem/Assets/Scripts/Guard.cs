using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : EnemyDetection
{
    [Header("GuardSettings")]
    public float headRotationSpeed;
    public float headRotateAmount;
    public float headRotateDelay;
    public float positionDistance;
    public NavMeshAgent agent;
    public GameObject target;
    public Vector3 lastKnowPosition;
    public Vector3[] path;
    private int currentPathIndex;
    private bool activeWalk = true;
    public float idleToWalkDelay;
    public float suspicionDetectMultiplier;

    public enum CurrentState { Idle, Following, Lost, Suspicious }
    public CurrentState currentState;

    public void FollowPath()
    {
        if (activeWalk)
        {
            if (Vector3.Distance(transform.position, path[currentPathIndex]) <= positionDistance)
                if (currentPathIndex == path.Length - 1)
                    currentPathIndex = 0;
                else
                    currentPathIndex++;

            agent.SetDestination(path[currentPathIndex]);
        }
    }

    public int GetClosestPathNode()
    {
        int lowest = 0;
        for (int i = 0; i < path.Length; i++)
            if (Vector3.Distance(transform.position, path[i]) <= Vector3.Distance(transform.position, path[lowest]))
                lowest = i;
        return lowest;
    }

    public void Update()
    {
        UpdateDetection();
        DisplayGradient();
        switch (currentState)
        {
            case CurrentState.Following:
                SetWalkPosition();
                agent.SetDestination(target.transform.position);
                Vector3 lookDir = new Vector3(target.transform.position.x, detectPosition.position.y, target.transform.position.z) - detectPosition.position;
                detectPosition.rotation = Quaternion.Lerp(detectPosition.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.deltaTime * headRotationSpeed);
                break;

            case CurrentState.Lost:
                if (Vector3.Distance(transform.position, lastKnowPosition) <= positionDistance)
                {
                    StartCoroutine(WalkDelay(idleToWalkDelay));
                    currentState = CurrentState.Suspicious;
                    currentPathIndex = GetClosestPathNode();
                }
                break;

            case CurrentState.Idle:
                FollowPath();
                break;

            case CurrentState.Suspicious:
                FollowPath();
                break;
        }
    }

    public IEnumerator WalkDelay(float time)
    {
        activeWalk = false;
        yield return new WaitForSeconds(time);
        activeWalk = true;
    }

    public IEnumerator HeadRotation()
    {
        bool right = true;
        float currentRotation = 0;
        while (currentState == CurrentState.Lost || currentState == CurrentState.Suspicious)
        {
            Vector3 rightDir = Quaternion.Euler(Vector3.up * headRotateAmount) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(Vector3.up * -headRotateAmount) * transform.forward;
            Quaternion lookRot = Quaternion.LookRotation(Vector3.Lerp(rightDir, leftDir, (currentRotation / 2f) + 0.5f), Vector3.up);
            detectPosition.rotation = Quaternion.Lerp(detectPosition.rotation, lookRot, Time.deltaTime * headRotationSpeed);
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
            lastKnowPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
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
                currentDetectLevel += Time.deltaTime * ((currentState == CurrentState.Suspicious) ? suspicionDetectMultiplier : 1f);
                if (currentDetectLevel >= detectedTime || currentState == CurrentState.Lost)
                {
                    target = CheckDetect()[0];
                    currentState = CurrentState.Following;
                }
            }
            else if (currentDetectLevel > 0 && (currentState == CurrentState.Idle || currentState == CurrentState.Suspicious))
                currentDetectLevel -= detectedDropTime * Time.deltaTime;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawRay(transform.position, Quaternion.Euler(Vector3.up * headRotateAmount) * transform.forward);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(Vector3.up * -headRotateAmount) * transform.forward);
        Gizmos.color = Color.blue;
        foreach (Vector3 pos in path)
            Gizmos.DrawSphere(pos, 0.3f);
    }

}
