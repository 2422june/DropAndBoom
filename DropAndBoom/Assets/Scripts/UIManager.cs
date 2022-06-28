using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public Slider DPHpBar { get; set; }
    public Slider BMHpBar { get; set; }

    public void AddBMHp(int damage)
    {
        GameManager.GM.PV.RPC("_AddBMHp", RpcTarget.All, damage);
    }

    public void AddDPHp(int damage)
    {
        GameManager.GM.PV.RPC("_AddDPHp", RpcTarget.All, damage);
    }

    [PunRPC]
    public void _AddBMHp(int damage)
    {
        BMHpBar.value += damage;
        if (BMHpBar.value > 3)
            BMHpBar.value = 3;
    }

    [PunRPC]
    public void _AddDPHp(int damage)
    {
        DPHpBar.value += damage;
        if (DPHpBar.value > 10)
            DPHpBar.value = 10;
    }
}
