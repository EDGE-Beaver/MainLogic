using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*=======================

[작성자] : 박준건

[소개]

* 본 매니저는 사운드 파일에 대한 전체적인 관리를 총괄하는 매니저임. 

[대략적 기능 소개]

1. Awake

매니저가 씬에 존재하면, SoundEffectFile - BgmAudioFile - VoiceAudioFile 세 개의 변수에 기입된 파일 경로를 읽고 파일들을 가져옴. 

[변수 소개]


==========================*/

/// <summary>
/// 사운드 파일을 가져와서 저장하고, 출력해주는 매니저입니다. 
/// <para>
/// 효과음, Bgm, 보이스 오디오 파일을 저장합니다. 
/// </para>
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("사용할 오디오 소스들(파일 경로로 기입)")]
    public List<string> SoundEffectFile = new List<string>();//효과음 파일
    public List<string> BgmAudioFile = new List<string>();//bgm 파일
    public List<string> VoiceAudioFile = new List<string>();//(만약 할 수 있다면) 더빙 파일

    [Header("실제 오디오가 저장되는 공간")]
    public List<AudioSource> SoundEffect = new List<AudioSource>();
    public List<AudioSource> BgmAudio = new List<AudioSource>();
    public List<AudioSource> VoiceAudio = new List<AudioSource>();

    [Header("오디오 재생에 사용할 게임 오브젝트들")]
    public GameObject SoundEffectListner;//효과음의 출력을 관할하는 파일
    public GameObject BgmListener;//bgm의 출력을 관할하는 파일
    public GameObject VoicListener;//(할 수 있다면) 더빙된 목소리의 출력을 관할하는 파일. 

    void Awake()
    {
        SoundEffectFileRead();
        BgmAudioFileRead();
        VoiceAudioFileRead();
    }

    /* ===========================
    파일 불러오는 공간

    =============================*/

    private void VoiceAudioFileRead()
    {
        throw new NotImplementedException();
    }

    private void BgmAudioFileRead()
    {
        throw new NotImplementedException();
    }

    private void SoundEffectFileRead()
    {
        foreach(var SeFileName in SoundEffectFile){
            AudioSource audioSource = Resources.Load<AudioSource>(SeFileName);
            SoundEffect.Add(audioSource);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
