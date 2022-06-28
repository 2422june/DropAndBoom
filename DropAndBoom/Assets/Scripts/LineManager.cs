using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager inst;

    [SerializeField]
    private RaycastHit[] hit;

    [SerializeField]
    private GameObject rayPos;
    
    private int cnt;

    private Vector3 vec;

    private void Awake()
    {
        inst = this;
    }

    public void Check()
    {
        cnt = 0;
        vec = rayPos.transform.position;

        hit = Physics.RaycastAll(vec, transform.right, 1000, 1<<3);

        Debug.DrawRay(vec, transform.right, Color.red, 1);

        if (hit.Length >= 10)
        {

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.CompareTag("Block"))
                {
                    PhotonNetwork.Destroy(hit[i].collider.gameObject);
                }
            }

            GameManager.GM.UIMNG.AddDPHp(3);
        }
    }

    public void OverCheck()
    {
        cnt = 0;
        vec = transform.position;
        vec.y += 15;

        hit = Physics.RaycastAll(vec, transform.right, 1000);


        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.CompareTag("Block"))
            {
                Debug.Log("Gameover");
            }
        }
    }
}
