using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    float fallSpeed;

    bool isBust, isFalling, isLanding;

    int line;

    Vector3 Vec;

    Rigidbody myRigid;

    void Start()
    {
        isFalling = false;
        isBust = false;
        isLanding = false;
        fallSpeed = 10f;
        line = 0;

        myRigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!isFalling)
            {
                myRigid.useGravity = true;

                Vec = Vector3.down;
                Vec *= fallSpeed;
                myRigid.velocity = Vec;

                isFalling = true;
            }
            else if(!isBust)
            {
                Vec = Vector3.down;
                Vec.y = fallSpeed * -5;
                myRigid.velocity = Vec;

                isBust = true;
            }
        }

        if(!isFalling)
        {
           if (Input.GetKeyDown(KeyCode.RightArrow) && line != 4)
            {
                line++;
                Vec = Vector3.zero;
                Vec.x = 1.3f;
                transform.position += Vec;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftArrow) && line != -4)
            {
                line--;
                Vec = Vector3.zero;
                Vec.x = -1.3f;
                transform.position += Vec;
            }
        }
        else
        {
            if(!isLanding)
            {
                if (transform.position.y <= -8)
                {
                    Vec = transform.position;
                    Vec.y = -8f;
                    transform.position = Vec;

                    CreateBlock();
                }
                else if(myRigid.velocity.y > -1)
                {
                    CreateBlock();
                }
            }

        }
    }


    void CreateBlock()
    {
        myRigid.velocity = Vector3.zero;

        Vec = Vector3.zero;
        Vec.y = 8.7f;

        Instantiate(Resources.Load<GameObject>("Prefabs/Block"), Vec, Quaternion.identity, null);

        isLanding = true;
    }
}
