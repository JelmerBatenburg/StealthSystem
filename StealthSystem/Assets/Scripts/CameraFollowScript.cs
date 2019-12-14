using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public float lerpSpeed;
    public float zoffset;
    public Transform followObject;

    //Update
    ///A really simple follow script
    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(followObject.position.x, transform.position.y, followObject.position.z + zoffset), Time.deltaTime * lerpSpeed);
    }
}
