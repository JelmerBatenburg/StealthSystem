using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : EnemyDetection
{
    public LayerMask guardMask;
    public float guardDetectRange;
    public GameObject callingDisplay;

    public void Update()
    {
        DetectEnemy();
    }

    public void DetectEnemy()
    {
        DisplayGradient();
        if (CheckDetect().Count > 0)
        {
            if (currentDetectLevel >= detectedTime)
            {
                Collider[] guards = Physics.OverlapSphere(transform.position, guardDetectRange, guardMask);
                callingDisplay.SetActive(true);
                foreach (Collider guard in guards)
                {
                    Guard currentGuard = guard.GetComponent<Guard>();
                    if(currentGuard.currentState == Guard.CurrentState.Idle || currentGuard.currentState == Guard.CurrentState.Suspicious)
                    {
                        currentGuard.lastKnowPosition = new Vector3(CheckDetect()[0].transform.position.x, currentGuard.transform.position.y, CheckDetect()[0].transform.position.z);
                        currentGuard.currentState = Guard.CurrentState.Lost;
                        currentGuard.agent.SetDestination(currentGuard.lastKnowPosition);
                        currentGuard.currentDetectLevel = currentGuard.detectedTime;
                    }
                }
            }
            else
            {
                currentDetectLevel += Time.deltaTime;
                callingDisplay.SetActive(false);
            }
        }
        else if (currentDetectLevel >= 0)
        {
            currentDetectLevel -= Time.deltaTime * detectedDropTime;
            callingDisplay.SetActive(false);
        }
    }
    
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, guardDetectRange);
    }
}
