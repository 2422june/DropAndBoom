using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager inst;

    [SerializeField]
    RaycastHit[] hit;
    
    int cnt;

    Vector3 vec;

    private void Awake()
    {
        inst = this;
    }

    public void Check()
    {
        cnt = 0;
        vec = transform.position;

        hit = Physics.RaycastAll(vec, transform.right, 1000);

        Debug.Log(hit.Length);

        if (hit.Length > 10)
        {
            Debug.Log("LineClear");

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.CompareTag("Block"))
                {
                    Destroy(hit[i].collider.gameObject);
                }
            }
        }
    }

    public void OverCheck()
    {
        cnt = 0;
        vec = transform.position;
        vec.y += 15;

        Debug.DrawRay(vec, transform.right, Color.red, 5);
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
