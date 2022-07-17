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
    public PhotonView PV;

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
    private GameObject LosePanel;
    [SerializeField]
    private GameObject ResultPanel;
    [SerializeField]
    private GameObject optionPanel;
    [SerializeField]
    private TMP_Text WaitingText;

    [SerializeField]
    private GameObject BMHp;
    [SerializeField]
    private GameObject DPHp;

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
    [SerializeField]
    private Slider BGMSlider;
    [SerializeField]
    private Slider EFTSlider;

    private bool isCanLeave;

    public static bool isDroper; // t = droper, f = boomer
    public static bool isPause;
    private bool isInGame;

    public static GameManager GM;
    public UIManager UIMNG;

    public enum Scene
    {
        title, loby, ingame
    };

    public Scene scene;

    private void Awake()
    {
        GM = this;
        PV = GetComponent<PhotonView>();
        SoundManager.soundMNG = GetComponent<SoundManager>();
        SoundManager.soundMNG.Init();
        UIMNG = GetComponent<UIManager>();
        PN.LocalPlayer.NickName = $"{Random.Range(0, 100)}";
        isCanLeave = true;
        SetBGMVolum();
        SetEFTVolum();
        SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.title);
        SetScene("title");
    }

    private void Start()
    {
        shakeOriginPos = Camera.main.transform.position;
    }

    private void SetScene(string target)
    {
        switch(target)
        {
            case "title":
                SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.title);
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

    public void ShowOptiopn()
    {
        if(isInGame && !isPause)
        {
            optionPanel.SetActive(true);
            PV.RPC("Pause", RpcTarget.All);
            PV.RPC("PauseOtherPlayer", RpcTarget.Others);
        }
    }
    public void DisableOptiopn()
    {
        optionPanel.SetActive(false);
        PV.RPC("Keep", RpcTarget.All);
        PV.RPC("KeepOtherPlayer", RpcTarget.Others);
    }

    public void GiveUp()
    {
        if(isDroper)
        {
            PV.RPC("DPHPToZero", RpcTarget.All);
        }
        else
        {
            PV.RPC("BMHPToZero", RpcTarget.All);
        }
    }

    [PunRPC]
    void BMHPToZero()
    {
        UIMNG.BMHpBar.value = 0;
    }

    [PunRPC]
    void DPHPToZero()
    {
        UIMNG.DPHpBar.value = 0;
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
        isDroper = true;
        if (PN.CurrentRoom.PlayerCount == PN.CurrentRoom.MaxPlayers)
        {
            WaitingText.text = "다른 참가자가 입장했습니다.";
            isCanLeave = false;
            isDroper = false;
            SoundManager.soundMNG.StopBGM();
        }
        //SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.inRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PN.CurrentRoom.PlayerCount == PN.CurrentRoom.MaxPlayers)
        {
            WaitingText.text = "다른 참가자가 입장했습니다.";
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
        isDroper = true;
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
        Debug.Log("leave");
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
        SetScene("ingame");
        StartCoroutine(InGame());
    }

    private IEnumerator InGame()
    {
        isPause = false;
        yield return one;
        yield return one;
        //loadingPanel.SetActive(true);
        roomPanel.SetActive(false);
        WaitingText.text = "다른 참가자를 기다리는 중";
        SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.enterRoom);
        yield return one;
        yield return one;

        loadingPanel.SetActive(false);
        inGamePanel.SetActive(true);
        isCanLeave = true;
        for (int i = 5; i >= 0; i--)
        {
            yield return one;
            countDown.text = $"{i}";
            SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.enterRoom);
        }

        yield return one;
        countDown.text = "Count Down End";
        SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.inGame);
        inGameObjects.SetActive(true);
        UIMNG.BMHpBar = BMHp.GetComponent<Slider>();
        UIMNG.DPHpBar = DPHp.GetComponent<Slider>();

        yield return one;
        countDown.text = "";
        if (isDroper)
        {
            PN.Instantiate("Prefabs/Block", Vector3.zero + (Vector3.up * 13), Quaternion.identity);
        }
        else
        {
            CreatePlayer();
        }
        isInGame = true;

        while (true)
        {
            if (UIMNG.BMHpBar.value <= 0)
            {
                SoundManager.soundMNG.StopBGM();
                ResultPanel.SetActive(true);
                if (isDroper)
                {
                    victoryPanel.SetActive(true);
                }
                else
                {
                    LosePanel.SetActive(true);
                }
                DisableOptiopn();
                break;
            }
            if (UIMNG.DPHpBar.value <= 0)
            {
                SoundManager.soundMNG.StopBGM();
                ResultPanel.SetActive(true);
                if (isDroper)
                {
                    LosePanel.SetActive(true);
                }
                else
                {
                    victoryPanel.SetActive(true);
                }
                DisableOptiopn();
                break;
            }
            yield return null;
        }

        PN.LeaveRoom();

        yield return one;

        while (true)
        {
            if (Input.anyKeyDown)
            {
                UIMNG.BMHpBar.value = 3;
                UIMNG.DPHpBar.value = 10;

                ResultPanel.SetActive(false);
                victoryPanel.SetActive(false);
                LosePanel.SetActive(false);
                inGameObjects.SetActive(false);
                inGamePanel.SetActive(false);
                DisableOptiopn();

                SoundManager.soundMNG.PlayEFT(SoundManager.eftClip.title);
                SoundManager.soundMNG.PlayBGM(SoundManager.bgmClip.title);

                SetScene("loby");
                break;
            }

            yield return null;
        }
    }

    [PunRPC]
    private void Pause()
    {
        isPause = true;
    }

    [PunRPC]
    private void Keep()
    {
        isPause = false;
    }

    [PunRPC]
    private void PauseOtherPlayer()
    {
        countDown.text = "상대가 게임을 정지 했습니다.";
    }

    [PunRPC]
    private void KeepOtherPlayer()
    {
        countDown.text = "";
    }

    public void RespwanPlayer(float t)
    {
        Invoke("CreatePlayer", t);
    }

    public void CreatePlayer()
    {
        PN.Instantiate("Prefabs/Boomer", Vector3.zero, Quaternion.Euler(Vector3.up * 90));
    }

    public void SetBGMVolum()
    {
        SoundManager.soundMNG.BGM.volume = BGMSlider.value;
    }

    public void SetEFTVolum()
    {
        SoundManager.soundMNG.EFTSources[1].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[2].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[3].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[4].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[5].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[6].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[7].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[8].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[9].volume = 30;//FTSlider.value;
        SoundManager.soundMNG.EFTSources[0].volume = 30;//EFTSlider.value;
    }

    private Vector3 shakePos;
    private Vector3 shakeOriginPos;
    private int shakeCnt;
    private WaitForSeconds shakeDelay = new WaitForSeconds(0.03f);

    [PunRPC]
    public IEnumerator CameraShake()
    {
        shakeCnt = Random.Range(3, 10);

        while(shakeCnt > 0)
        {
            shakePos.x = Random.Range(-0.3f, 0.3f);
            shakePos.y = Random.Range(-0.3f, 0.3f);
            Camera.main.transform.position += shakePos;
            yield return shakeDelay;
            shakeCnt--;
            Camera.main.transform.position = shakeOriginPos;
        }
    }
}
