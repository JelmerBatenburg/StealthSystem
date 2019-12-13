using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetection : MonoBehaviour
{
    [Header("DetectionArea")]
    public float detectWidth;
    public float detectRange;

    [Header("DestectSettings")]
    public Transform detectPosition;

    [Header("Settings")]
    public LayerMask detectionMask;
    public float detectedTime;
    public float detectedDropTime;
    [HideInInspector]
    public float currentDetectLevel;

    [Header("DetectionDisplay")]
    public Image fillDisplay;
    public Gradient gradientDisplay;

    //DisplayGradient
    ///This will show a bar that will display the level of detection
    ///It will fill up the bar and change color the more it has detected a target
    public void DisplayGradient()
    {
        fillDisplay.gameObject.SetActive(currentDetectLevel > 0);
        fillDisplay.transform.LookAt(Camera.main.transform.position);
        fillDisplay.fillAmount = (1f / detectedTime) * currentDetectLevel;
        fillDisplay.color = gradientDisplay.Evaluate(fillDisplay.fillAmount);
    }

    //CheckDetect
    ///This will detect all targtets within it's field of view
    ///It will return the targets in a list of GameObjects
    public List<GameObject> CheckDetect()
    {
        ///Detects all the targets within an area
        Collider[] detectedColliders = Physics.OverlapSphere(detectPosition.position, detectRange, detectionMask);
        List<GameObject> detectedPlayers = new List<GameObject>();

        foreach (Collider col in detectedColliders)
        {
            ///Calculates the direction from the camera towards the target
            Vector3 dir = col.ClosestPoint(detectPosition.position) - detectPosition.position;
            RaycastHit hit = new RaycastHit();
            ///Checks if the direction of the player is in the field of view by checking if the angle between the forward direction and the direction of the target lower is than the detectwidth
            if (Mathf.Abs(Vector3.Angle(dir, detectPosition.forward)) <= detectWidth)
                ///Checks if there isn't an object in front of the target
                if (Physics.Raycast(detectPosition.position, dir, out hit) && hit.collider == col && hit.transform.GetComponent<PlayerDetection>().CanBeDetected())
                {
                    detectedPlayers.Add(col.gameObject);
                    Debug.DrawLine(detectPosition.position, col.transform.position, Color.red);
                }
        }
        ///returns the players
        return detectedPlayers;
    }

    //gizmos
    ///Shows the fov in the editor
    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawRay(detectPosition.position, (Quaternion.Euler(detectPosition.up * detectWidth) * detectPosition.forward) * detectRange);
        Gizmos.DrawRay(detectPosition.position, (Quaternion.Euler(-detectPosition.up * detectWidth) * detectPosition.forward) * detectRange);
    }
}
