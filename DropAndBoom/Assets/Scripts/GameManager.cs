using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class GameManager : MonoBehaviourPunCallbacks
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

    [SerializeField]
    GameObject CustomRoomInputField;
    [SerializeField]
    GameObject IDBox;
    [SerializeField]
    GameObject EnterErrorBox;

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
                if (scene == Scene.title)
                {
                    PN.ConnectUsingSettings();
                }
                scene = Scene.loby;
                CustomRoomInputField.SetActive(false);
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

    private void OnLoby()
    {

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

    public void OnClickEnterCustomRoom()
    {
        CustomRoomInputField.SetActive(true);
    }

    public void InputCustomRoomID()
    {
        Debug.Log("Create");
        PN.JoinRoom(IDBox.GetComponent<TextMesh>().text, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowErrorBox(true);
    }

    public void OnClickEnterCustomRoomCancle()
    {
        CustomRoomInputField.SetActive(false);
    }

    public void OnClickRandomCustomRoom()
    {

    }

    public void ShowErrorBox(bool show)
    {
        EnterErrorBox.SetActive(show);
        if(show)
            Invoke("ShowErrorBox(false)", 1f);

    }
}
