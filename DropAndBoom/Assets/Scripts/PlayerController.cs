using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed, h;
    Vector3 dir, rotate;

    void Start()
    {
        speed = 10f;
        dir = Vector3.zero;

        rotate = Vector3.zero;
        rotate.y = 90;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            h = 1;

            rotate.y = 90 * h;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            h = -1;

            rotate.y = h * 90;
        }
        else
        {
            h = 0;
        }

        dir.x = h * 1.3f;
        transform.position += dir;
        transform.rotation = Quaternion.Euler(rotate);
    }
}
