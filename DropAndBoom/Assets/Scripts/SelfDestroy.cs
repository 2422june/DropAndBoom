using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SelfDestroy : MonoBehaviour
{
    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            Invoke("_Destroy", 0.5f);
        }
    }

    void _Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
