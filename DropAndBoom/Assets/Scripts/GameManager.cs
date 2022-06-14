using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject TitlePanel;
    [SerializeField]
    GameObject LobyPanel;
    [SerializeField]
    GameObject IngamePanel;
    [SerializeField]
    GameObject victoryPanel;
    [SerializeField]
    GameObject LosePanel;

    public enum Scene
    {
        title, ingame, loby, victory, lose
    };

    public Scene scene;

    private void Awake()
    {
        SetScene("title");
    }

    public void SetScene(string target)
    {
        switch(target)
        {
            case "title":
                scene = Scene.title;
                StartCoroutine(TitleInput());
                break;

            case "loby":
                scene = Scene.loby;
                break;

            case "ingame":
                scene = Scene.ingame;
                break;

            case "victory":
                scene = Scene.victory;
                break;

            case "lose":
                scene = Scene.lose;
                break;

            default:
                Debug.Log("Not found Scene");
                break;
        }

        TitlePanel.SetActive(scene == Scene.title);
        LobyPanel.SetActive(scene == Scene.loby);
        //IngamePanel.SetActive(scene == Scene.ingame);
        //victoryPanel.SetActive(scene == Scene.victory);
        //LosePanel.SetActive(scene == Scene.lose);
    }

    private IEnumerator TitleInput()
    {
        while(true)
        {
            if (Input.anyKeyDown)
            {
                SetScene("loby");
                break;
            }

            yield return null;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
