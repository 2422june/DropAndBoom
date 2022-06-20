using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class GameManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    private GameObject titlePanel;
    [SerializeField]
    private GameObject lobyPanel;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject inGamePanel;
    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private GameObject losePanel;

    [SerializeField]
    private GameObject enterErrorBox;
    [SerializeField]
    private GameObject quitBoard;
    [SerializeField]
    private TMP_Text roomInfo;

    [SerializeField]
    private GameObject inGameObjects;
    [SerializeField]
    private TMP_Text countDown;

    private enum Scene
    {
        title, loby, ingame
    };

    private Scene scene;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        SetScene("title");
        PN.LocalPlayer.NickName = $"{Random.Range(0, 100)}";
    }

    private void SetScene(string target)
    {
        switch(target)
        {
            case "title":
                scene = Scene.title;
                StartCoroutine(TitleInput());
                break;

            case "loby":
                loadingPanel.SetActive(false);
                scene = Scene.loby;
                break;

            case "ingame":
                scene = Scene.ingame;
                break;

            default:
                Debug.Log("Not found Scene");
                break;
        }

        titlePanel.SetActive(scene == Scene.title);
        lobyPanel.SetActive(scene == Scene.loby);
    }

    private IEnumerator TitleInput()
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                PN.ConnectUsingSettings();
                break;
            }

            yield return null;
        }
    }

    private IEnumerator GameFadeOut()
    {
        while (quitBoard.transform.localPosition.y > 0)
        {
            quitBoard.transform.position += Vector3.down * Time.deltaTime * 1080f;

            yield return null;
        }
        quitBoard.transform.position = Vector3.zero;
        PN.Disconnect();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        PN.JoinLobby();
        SetScene("loby");
    }

    public void OnClickEnterRandomRoom()
    {
        lobyPanel.SetActive(false);
        loadingPanel.SetActive(true);
        PN.JoinRandomOrCreateRoom(
            null, 2, Photon.Realtime.MatchmakingMode.RandomMatching,
            null, null, $"{Random.Range(0, 1000)}",
            new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        loadingPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomInfo.text = $"방 번호 : {PN.CurrentRoom.Name}";
        if (PN.CurrentRoom.PlayerCount == PN.CurrentRoom.MaxPlayers)
        {
            loadingPanel.SetActive(true);
            roomPanel.SetActive(false);
            PV.RPC("EnterGame", RpcTarget.All, null);
            StartCoroutine(InGame());
        }
    }

    public override void OnCreatedRoom()
    {
        roomInfo.text = $"방 번호 : {PN.CurrentRoom.Name}";
        loadingPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowErrorBox(true);
    }

    public void OnClickQuitGame()
    {
        if(!quitBoard.active)
        {
            PN.Disconnect();
            quitBoard.SetActive(true);
            StartCoroutine(GameFadeOut());
        }
    }

    public void LeaveRoom()
    {
        roomPanel.SetActive(false);
        loadingPanel.SetActive(true);
        PN.LeaveRoom();
    }

    public void ShowErrorBox(bool show = false)
    {
        enterErrorBox.SetActive(show);
        if(show)
            Invoke("ShowErrorBox", 3f);
    }


    WaitForSeconds one = new WaitForSeconds(1f);

    [PunRPC]
    private void EnterGame()
    {
        StartCoroutine(InGame());
    }

    private IEnumerator InGame()
    {
        loadingPanel.SetActive(false);
        inGamePanel.SetActive(true);
        for (int i = 5; i > 0; i--)
        {
            yield return one;
            countDown.text = $"{i}";
        }

        while(true)
        {
            inGameObjects.SetActive(true);
            countDown.text = "Count Down End";
            break;
        }
    }
}
