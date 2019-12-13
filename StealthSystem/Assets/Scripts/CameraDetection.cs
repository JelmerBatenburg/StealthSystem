using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : EnemyDetection
{
    public LayerMask guardMask;
    public float guardDetectRange;
    public GameObject callingDisplay;

    //Update
    public void Update()
    {
        DisplayGradient();
        DetectEnemy();
    }

    //Detect Enemy
    ///Checks for a target
    public void DetectEnemy()
    {
        ///Checks for something in it's field of view
        if (CheckDetect().Count > 0)
        {
            ///When the detection level is high enough it will send all nearby guards to the target
            if (currentDetectLevel >= detectedTime)
            {
                ///Gets all targets in the area
                Collider[] guards = Physics.OverlapSphere(transform.position, guardDetectRange, guardMask);
                ///Indicates that it is calling something
                callingDisplay.SetActive(true);
                ///Calls each guard
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
                ///Adds a detect value
                currentDetectLevel += Time.deltaTime;
                callingDisplay.SetActive(false);
            }
        }
        else if (currentDetectLevel >= 0)
        {
            ///Lowers the detect value
            currentDetectLevel -= Time.deltaTime * detectedDropTime;
            callingDisplay.SetActive(false);
        }
    }
    
    //Gizmos
    ///Shows the guard call area
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, guardDetectRange);
    }
}
