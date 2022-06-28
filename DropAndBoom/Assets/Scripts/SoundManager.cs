using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundMNG;

    public AudioSource BGM;
    public List<AudioSource> EFTSources = new List<AudioSource>();

    [SerializeField]
    private List<AudioClip> EftClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> bgmClips = new List<AudioClip>();

    public enum eftClip
    {
        title, gameExit, enterRoom, cant, bmHit
    };
    public enum bgmClip
    {
        title, main, inGame
    };

    int index = 0;

    public void Init()
    {
        BGM = GetComponent<AudioSource>();

        for(index = 0; index < 10; index++)
        {
            EFTSources.Add(gameObject.AddComponent<AudioSource>());
        }
        index--;
    }
    
    public void PlayEFT(eftClip num)
    {
        EFTSources[index].clip = EftClips[(int)num];
        EFTSources[index].Play();
        index++;
        index %= 5;
    }

    public void PlayBGM(bgmClip num)
    {
        while (BGM.clip != bgmClips[(int)num])
            BGM.clip = bgmClips[(int)num];

        BGM.Play();
    }

    public void StopBGM()
    {
        BGM.Stop();
    }
}
