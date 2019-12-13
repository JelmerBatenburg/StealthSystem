using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : EnemyDetection
{
    [Header("Head Movment")]
    public float headRotationSpeed;
    public float headRotateAmount;

    [Header("Movement")]
    public float positionDistance;
    [HideInInspector]
    public Vector3 lastKnowPosition;
    public Vector3[] path;
    private int currentPathIndex;
    private bool activeWalk = true;

    [Header("Other Settings")]
    public float idleToWalkDelay;
    public float suspicionDetectMultiplier;
    public NavMeshAgent agent;
    public GameObject target;

    public enum CurrentState { Idle, Following, Lost, Suspicious }
    public CurrentState currentState;

    //Update
    public void Update()
    {
        UpdateDetection();
        DisplayGradient();
        StateCheck();
    }

    //StateCheck
    ///Does the right thing for each state the guard is in
    public void StateCheck()
    {
        switch (currentState)
        {
            //Following
            ///First saves the position of the target it is following and then sets it's agent destination to the target
            ///It will also look at the target so it will stay in its field of view
            case CurrentState.Following:
                if (target)
                    lastKnowPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                agent.SetDestination(target.transform.position);
                Vector3 lookDir = new Vector3(target.transform.position.x, detectPosition.position.y, target.transform.position.z) - detectPosition.position;
                detectPosition.rotation = Quaternion.Lerp(detectPosition.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.deltaTime * headRotationSpeed);
                break;

            //Lost
            ///It will walk to the last known position and when it is near the position it will go to the suspicious state and go back to following it's path
            case CurrentState.Lost:
                if (Vector3.Distance(transform.position, lastKnowPosition) <= positionDistance)
                {
                    StartCoroutine(WalkDelay(idleToWalkDelay));
                    currentState = CurrentState.Suspicious;
                    currentPathIndex = GetClosestPathNode();
                }
                break;

            //Idle and Suspicious
            ///It will follow the path
            case CurrentState.Idle:
                FollowPath();
                break;

            case CurrentState.Suspicious:
                FollowPath();
                break;
        }
    }

    //WalkDelay
    ///Starts a delay for following the path
    public IEnumerator WalkDelay(float time)
    {
        activeWalk = false;
        yield return new WaitForSeconds(time);
        activeWalk = true;
    }

    //GetClosestPathNode
    ///Will get the closest position from the pathlist and will continue it's path from that position
    public int GetClosestPathNode()
    {
        int lowest = 0;
        for (int i = 0; i < path.Length; i++)
            if (Vector3.Distance(transform.position, path[i]) <= Vector3.Distance(transform.position, path[lowest]))
                lowest = i;
        return lowest;
    }

    //FollowPath
    ///Follows a given path
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

    //HeadRotation
    ///This will be active when the guard is either lost or suspicious
    public IEnumerator HeadRotation()
    {
        bool right = true;
        float currentRotation = 0;
        while (currentState == CurrentState.Lost || currentState == CurrentState.Suspicious)
        {
            ///Gets the right look direction aswell as the rightLookDirection
            Vector3 rightDir = Quaternion.Euler(Vector3.up * headRotateAmount) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(Vector3.up * -headRotateAmount) * transform.forward;
            ///Calculates the rotation
            Quaternion lookRot = Quaternion.LookRotation(Vector3.Lerp(rightDir, leftDir, (currentRotation / 2f) + 0.5f), Vector3.up);
            ///Rotates to the right rotation with a lerp
            detectPosition.rotation = Quaternion.Lerp(detectPosition.rotation, lookRot, Time.deltaTime * headRotationSpeed);
            ///Checks what direction it will look in
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

    //Update Detection
    ///Looks for the target
    public void UpdateDetection()
    {
        ///When it is following the target and it has lost it it will set the state to lost and go to the last known position.
        if (target && currentState == CurrentState.Following)
            if (!CheckDetect().Contains(target))
            {
                currentState = CurrentState.Lost;
                StartCoroutine(HeadRotation());
                target = null;
            }

        ///When it doesn't have a target it will check if it has a target in its field of view
        if (!target)
            if (CheckDetect().Count > 0)
            {
                ///It will add the a value to the detect level
                currentDetectLevel += Time.deltaTime * ((currentState == CurrentState.Suspicious) ? suspicionDetectMultiplier : 1f);
                ///When the current detect level is over a certain value it will set the target in the field of view as the target
                if (currentDetectLevel >= detectedTime || currentState == CurrentState.Lost)
                {
                    target = CheckDetect()[0];
                    currentState = CurrentState.Following;
                }
            }
            ///Lowest the value when it doesn't have a target in it's field of view
            else if (currentDetectLevel > 0 && (currentState == CurrentState.Idle || currentState == CurrentState.Suspicious))
                currentDetectLevel -= detectedDropTime * Time.deltaTime;
    }

    //Gizmos
    ///Draws to lookRays and the path positions
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
