using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    float fallSpeed;

    bool isBust, isFalling, isLanding;

    int posX;
    static int num;
    public int myNum;

    Vector3 Vec;

    Rigidbody myRigid;

    public bool isDestroy;
    private PhotonView PV;
    private GameObject particle;

    void Start()
    {
        if (!GameManager.isDroper)
        {
            return;
        }
        isFalling = false;
        isBust = false;
        isLanding = false;
        fallSpeed = 10f;
        posX = 0;

        myRigid = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!GameManager.isDroper)
        {
            return;
        }

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
            else if(!isBust && !isLanding)
            {
                Vec = Vector3.down;
                Vec.y = fallSpeed * -5;
                myRigid.velocity = Vec;

                isBust = true;
            }
        }

        if(!isFalling)
        {
           if (Input.GetKeyDown(KeyCode.RightArrow) && posX != 4)
            {
                posX++;
                Vec = Vector3.right;
                transform.position += Vec;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftArrow) && posX != -5)
            {
                posX--;
                Vec = Vector3.left;
                transform.position += Vec;
            }
        }
        else
        {
            if(!isLanding)
            {
                if (transform.position.y <= -3)
                {
                    Vec = transform.position;
                    Vec.y = -3f;
                    transform.position = Vec;

                    myRigid.velocity = Vector3.zero;
                    CreateBlock();
                }
                else if (myRigid.velocity.y > -1)
                {
                    myRigid.velocity = Vector3.zero;
                    CreateBlock();
                }
            }
        }

        if(isDestroy)
        {
            particle = PhotonNetwork.Instantiate("Prefabs/Magic", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }

        if(GameManager.GM.scene != GameManager.Scene.ingame)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void CreateBlock()
    {
        if (!GameManager.isDroper)
            return;

        GameManager.GM.PV.RPC("CameraShake", RpcTarget.All);

        Vec = Vector3.zero;
        Vec.y = 13f;

        PhotonNetwork.Instantiate("Prefabs/Block", Vec, Quaternion.identity);
        //Instantiate(Resources.Load<GameObject>("Prefabs/Block"), Vec, Quaternion.identity, null);

        LineManager.inst.Check();
        LineManager.inst.OverCheck();

        isLanding = true;
    }

    [PunRPC]
    private void CameraShake()
    {
        GameManager.GM.CameraShake();
    }
}
