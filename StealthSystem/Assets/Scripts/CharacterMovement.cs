using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed;
    public Vector3 force;
    public float drag;
    public float checkRange;
    public float checkWidth;
    public LayerMask mask;

    public void Update()
    {
        force += new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * movementSpeed;
        force = Vector3.Lerp(force, Vector3.zero, Time.deltaTime * drag);
        CheckForWalls();
        transform.Translate(force * Time.deltaTime);
    }

    public void CheckForWalls()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(transform.position, checkWidth, transform.forward, out hit, checkRange, mask) && force.z > 0)
            force.z = 0;
        if (Physics.SphereCast(transform.position, checkWidth, -transform.forward, out hit, checkRange, mask) && force.z < 0)
            force.z = 0;

    }
}
