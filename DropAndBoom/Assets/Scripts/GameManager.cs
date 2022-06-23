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

    private bool isCanLeave;

    private enum Scene
    {
        title, loby, ingame
    };

    private Scene scene;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        SoundManager.soundMNG = GetComponent<SoundManager>();
        SetScene("title");
        PN.LocalPlayer.NickName = $"{Random.Range(0, 100)}";
        SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.title);
        isCanLeave = true;
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
                SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.title);
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
        SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.title);
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
            isCanLeave = false;
            SoundManager.soundMNG.StopBGM();
        }
        //SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.inRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PN.CurrentRoom.PlayerCount == PN.CurrentRoom.MaxPlayers)
        {
            isCanLeave = false;
            SoundManager.soundMNG.StopBGM();
            PV.RPC("EnterGame", RpcTarget.All, null);
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
            SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.gameExit);
            PN.Disconnect();
            quitBoard.SetActive(true);
            StartCoroutine(GameFadeOut());
        }
    }

    public void LeaveRoom()
    {
        if(isCanLeave)
        {
            SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.gameExit);
            roomPanel.SetActive(false);
            loadingPanel.SetActive(true);
            PN.LeaveRoom();
        }
        else
        {
            SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.cant);
        }
        //SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.title);
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
        yield return one;
        yield return one;
        //loadingPanel.SetActive(true);
        roomPanel.SetActive(false);
        SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.enterRoom);
        yield return one;
        yield return one;

        loadingPanel.SetActive(false);
        inGamePanel.SetActive(true);
        for (int i = 5; i >= 0; i--)
        {
            yield return one;
            countDown.text = $"{i}";
            SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.enterRoom);
        }

        yield return one;

        while (true)
        {
            SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.inGame);
            inGameObjects.SetActive(true);
            countDown.text = "Count Down End";
            break;
        }
    }
}
