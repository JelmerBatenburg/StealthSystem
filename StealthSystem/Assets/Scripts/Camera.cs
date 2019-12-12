using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : EnemyDetection
{
    public LayerMask guardMask;
    public float guardDetectRange;

    public void Update()
    {
        DetectEnemy();
    }

    public void DetectEnemy()
    {
        if (CheckDetect().Count > 0)
        {
            if (currentDetectLevel >= detectedTime)
            {
                Collider[] guards = Physics.OverlapSphere(transform.position, guardDetectRange, guardMask);
                foreach (Collider guard in guards)
                {
                    Guard currentGuard = guard.GetComponent<Guard>();
                    if(currentGuard.currentState == Guard.CurrentState.Idle || currentGuard.currentState == Guard.CurrentState.Suspicious)
                    {
                        currentGuard.lastKnowPosition = new Vector3(CheckDetect()[0].transform.position.x, currentGuard.transform.position.y, CheckDetect()[0].transform.position.z);
                        currentGuard.currentState = Guard.CurrentState.Lost;
                        currentGuard.agent.SetDestination(currentGuard.lastKnowPosition);
                    }
                }
            }
            else
                currentDetectLevel += Time.deltaTime;
        }
        else if (currentDetectLevel >= 0)
            currentDetectLevel -= Time.deltaTime * detectedDropTime;
    }
    
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(transform.position, guardDetectRange);
    }
}
