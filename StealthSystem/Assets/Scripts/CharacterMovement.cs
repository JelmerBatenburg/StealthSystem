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

    //Update
    public void Update()
    {
        Move();
    }

    //Move
    ///Detects the input and applies the force after checking for walls
    public void Move()
    {
        force += new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * movementSpeed;
        force = Vector3.Lerp(force, Vector3.zero, Time.deltaTime * drag);
        CheckForWalls();
        transform.Translate(force * Time.deltaTime);
    }

    //CheckForWalls
    ///Will search for the walls and chenges the force
    public void CheckForWalls()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(transform.position, checkWidth, transform.forward, out hit, checkRange, mask) && force.z > 0)
            force.z = 0;
        if (Physics.SphereCast(transform.position, checkWidth, -transform.forward, out hit, checkRange, mask) && force.z < 0)
            force.z = 0;
        if (Physics.SphereCast(transform.position, checkWidth, transform.right, out hit, checkRange, mask) && force.x > 0)
            force.x = 0;
        if (Physics.SphereCast(transform.position, checkWidth, -transform.right, out hit, checkRange, mask) && force.x < 0)
            force.x = 0;

    }
}
