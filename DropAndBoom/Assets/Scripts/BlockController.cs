using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    float fallSpeed;

    bool isBust;

    Vector3 Vec;

    Rigidbody myRigid;

    void Start()
    {
        isBust = false;
        fallSpeed = 10f;

        myRigid = GetComponent<Rigidbody>();

        Vec = Vector3.down;
        Vec *= fallSpeed;

        myRigid.velocity = Vec;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isBust)
        {
            Vec = Vector3.down;
            Vec.y = fallSpeed * -5;
            myRigid.velocity = Vec;

            isBust = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vec = Vector3.zero;
            Vec.x = 1.2f;
            transform.position += Vec;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vec = Vector3.zero;
            Vec.x = -1.2f;
            transform.position += Vec;
        }

        if (transform.position.y <= -3.79f)
        {
            Vec = transform.position;
            Vec.y = -3.79f;

            myRigid.velocity = Vector3.zero;

            myRigid.useGravity = false;
        }
    }
}
