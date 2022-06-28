using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float h;
    private Vector3 dir, rotate;
    private float posX;
    private Rigidbody myRigid;
    private Ray ray;
    private RaycastHit hit;
    private PhotonView PV;


    void Start()
    {
        if(GameManager.isDroper)
        {
            return;
        }
        PV = GetComponent<PhotonView>();
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
            Debug.Log("Shot");
            ray.direction = Vector3.right * dir.x;
            //ray.origin += Vector3.up * 03f;
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);
            if (Physics.Raycast(ray, out hit, 1f, 1 << 3))
            {
                Debug.Log("Hit");
                GameManager.GM.UIMNG.AddBMHp(1);
                GameManager.GM.UIMNG.AddDPHp(-1);
                GameObject target = hit.collider.gameObject;
                
                
                PV.RPC("BlockDestroy", RpcTarget.All, target.GetComponent<PhotonView>().ViewID);
            }
        }

        ray.origin = transform.position;
        ray.direction = Vector3.up;
        if (Physics.Raycast(ray, 1f, 1 << 3))
        {
            GameManager.GM.UIMNG.AddBMHp(-1);
            GameManager.GM.RespwanPlayer(3);
            PhotonNetwork.Destroy(gameObject);
        }

        myRigid.velocity = (Vector3.right * h * 10) + (Vector3.up * myRigid.velocity.y);

        if (GameManager.GM.scene != GameManager.Scene.ingame)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void BlockDestroy(int id)
    {
        PhotonView.Find(id).GetComponent<BlockController>().isDestroy = true;
        //target
    }
}
