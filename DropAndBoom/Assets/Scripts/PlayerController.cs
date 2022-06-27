using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float h;
    Vector3 dir, rotate;
    float posX;
    Rigidbody myRigid;
    Ray ray;
    RaycastHit hit;
    bool iscanJump;

    void Start()
    {
        if(GameManager.isDroper)
        {
            return;
        }
        dir = Vector3.zero;

        rotate = Vector3.zero;
        rotate.y = 90;

        myRigid = GetComponent<Rigidbody>();

        ray = new Ray(transform.position, Vector3.right);
    }


    void Update()
    {
        if (GameManager.isDroper)
        {
            return;
        }
        posX = transform.position.x;

        Debug.DrawRay(transform.position + Vector3.up, Vector3.right * dir.x, Color.red, 1f);

        h = Input.GetAxisRaw("Horizontal");

        if ((h > 0 && posX >= 3.8f) || (h < 0 && posX <= -4.8f))
        {
            h = 0;
        }

        if(h != 0)
        {
            rotate.y = h * 90;
            dir.x = h;
            transform.rotation = Quaternion.Euler(rotate);
        }

        ray.origin = transform.position;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ray.direction = Vector3.right * dir.x;
            if (Physics.Raycast(ray, out hit, 1f, 1 << 3))
            {
                hit.collider.GetComponent<BlockController>().DoDestroy();
            }
        }

        ray.direction = Vector3.up;
        if (Physics.Raycast(ray, 1f, 1 << 3))
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(iscanJump)
            {
                myRigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
                iscanJump = false;
            }
        }

        ray.direction = Vector3.down;
        if(Physics.Raycast(ray, out hit, 0.1f, 1<<3))
        {
            iscanJump = true;
        }

        myRigid.velocity = (Vector3.right * h * 10) + (Vector3.up * myRigid.velocity.y);
    }
}
