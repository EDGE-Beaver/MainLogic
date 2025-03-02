using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;//이게 대체 왜 필요한걸까?

/*=======================

[작성자] : 박준건

[소개]

* 본 매니저는 사운드 파일에 대한 전체적인 관리를 총괄하는 매니저임. 

[대략적 기능 소개]

1. Awake

매니저가 씬에 존재하면, SoundEffectFile - BgmAudioFile - VoiceAudioFile 세 개의 변수에 기입된 파일 경로를 읽고 파일들을 가져옴. 

2. SetCurrent -> PlayCurrent
= 다이얼로그 매니저, 기타 등등에서 이 코드를 활용할 때, 우리는 SetCurrent로 현재 필요한 오디로를 설정하고 PlayCurrent로 그것을 출력하면 됨. 
만약 CurrentFile에 변경점이 없다면 PlayCurrent를 유지하면 된다. 

[변수 소개]

1. 기본적인 파일들의 이름이 들어가는 것은 list로 선언함. 

2. 파일 이름이 존재하는지 검사하는 로직은 HashSet을 이용함(탐색이 빠름)

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

    /// <summary>
    /// 중복 검사를 위한 SE의 해시셋 자료구조
    /// </summary>
    private HashSet<string> SoundEffectFileName = new HashSet<string>();

    /// <summary>
    /// 현재 효과음 파일, 재생시 이 파일명에 적힌 파일로 동작함. 
    /// </summary>
    private string CurrentSeFile;

    public List<string> BgmAudioFile = new List<string>();//bgm 파일

    /// <summary>
    /// 중복 검사용 자료구조
    /// </summary>
    private HashSet<string> BgmAudioFileName = new HashSet<string>();

    /// <summary>
    /// 현재 Bgm 파일, Bgm 재생시 이 파일명에 적힌 파일로 동작함. 
    /// </summary>
    private string CurrentBgmFile;

    /// <summary>
    /// 보이스 파일의 위치가 저장되는 곳
    /// </summary>
    public List<string> VoiceAudioFile = new List<string>();//(만약 할 수 있다면) 더빙 파일

    /// <summary>
    /// 보이스 파일의 이름 중복 검사를 위한 해시셋
    /// </summary>
    private HashSet<string> VoiceAudioFileName = new HashSet<string>();

    /// <summary>
    /// 현재 재생되고 있는 보이스 파일은 누구의 것인가?
    /// </summary>
    private string CurrentVoiceFile;

    [Header("실제 오디오가 저장되는 공간")]
    public Dictionary<string, AudioClip> SoundEffect = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> BgmAudio = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> VoiceAudio = new Dictionary<string, AudioClip>();

    [Header("오디오 재생에 사용할 게임 오브젝트들")]
    public GameObject SoundEffectListner;//효과음의 출력을 관할하는 오브젝트
    private AudioSource SeAudioSource;//효과음 출력 관할하는 오브젝트의 오디오 소스.

    public GameObject BgmListener;//bgm의 출력을 관할하는 오브젝트
    private AudioSource BgmAudioSource;//Bgm 출력을 관할하는 오브젝트의 오디오 소스

    public GameObject VoicListener;//(할 수 있다면) 더빙된 목소리의 출력을 관할하는 오브젝트
    private AudioSource VoiceAudioSource;//보이스 출력 관할하는 오브젝트의 오디오 소스

    void Awake()
    {
        SoundEffectFileRead();//파일 불러오고
        if(SoundEffectListner == null && GameObject.Find("SeAudioSource") == null){
            //리스터 연결, 리스너가 없는 상황 확인. 
            Debug.LogError("SeAudioSource가 비어 있습니다! 생성해서 연결해주세요");
        }else{
            SeAudioSource = SoundEffectListner.GetComponent<AudioSource>();
        }


        BgmAudioFileRead();
         if(BgmListener == null && GameObject.Find("BgmAudioSource") == null){
            //리스터 연결, 리스너가 없는 상황 확인. 
            Debug.LogError("BgmAudioSource가 비어 있습니다! 생성해서 연결해주세요");
        }else{
            BgmAudioSource = BgmListener.GetComponent<AudioSource>();
        }

        VoiceAudioFileRead();
        if(VoicListener == null && GameObject.Find("VoiceAudioSource") == null){
            //리스터 연결, 리스너가 없는 상황 확인. 
            Debug.LogError("VoiceAudioSource가 비어 있습니다! 생성해서 연결해주세요");
        }else{
            VoiceAudioSource = BgmListener.GetComponent<AudioSource>();
        }
    
    }

    /*[파일 불러오는 로직]*/

    /// <summary>
    /// 보이스 파일을 불러오는 부분입니다. 
    /// </summary>
    private void VoiceAudioFileRead()
    {
        foreach(var VoiceFileName in VoiceAudioFile){
            AudioClip AudioClip  = Resources.Load<AudioClip>(VoiceFileName);

            if(AudioClip == null){
                Debug.LogError("VoiceAudioFileRead에서의 오류");
                Debug.LogError("오디오 소스가 null입니다."); 
            }

            VoiceAudio.Add(System.IO.Path.GetFileName(VoiceFileName), AudioClip);
            
            VoiceAudioFileName.Add(System.IO.Path.GetFileName(VoiceFileName));

        }
    }

    /// <summary>
    /// 전체 Bgm 파일을 읽어옵니다. 
    /// </summary>
    private void BgmAudioFileRead()
    {
        foreach(var BgmFileName in BgmAudioFile){
            AudioClip AudioClip  = Resources.Load<AudioClip>(BgmFileName);

            if(AudioClip == null){
                Debug.LogError("BgmAudioFileRead에서의 오류");
                Debug.LogError("오디오 소스가 null입니다."); 
            }

            BgmAudio.Add(System.IO.Path.GetFileName(BgmFileName), AudioClip);
            
            BgmAudioFileName.Add(System.IO.Path.GetFileName(BgmFileName));
        }
    }

    /// <summary>
    /// 전체 사운드 이펙트 파일을 읽어옵니다.
    /// </summary>
    private void SoundEffectFileRead()
    {
        foreach(var SeFileName in SoundEffectFile){
            AudioClip AudioClip  = Resources.Load<AudioClip>(SeFileName);

            if(AudioClip == null){
                Debug.LogError("SoundEffectFileRead에서의 오류");
                Debug.LogError("오디오 소스가 null입니다."); 
            }

            SoundEffect.Add(System.IO.Path.GetFileName(SeFileName), AudioClip);
            
            SoundEffectFileName.Add(System.IO.Path.GetFileName(SeFileName));
        }
    }

    /*[설정 부분]*/

    /// <summary>
    /// 현재 효과음을 설정합니다
    /// </summary>
    /// <param name="Name">효과음의 이름입니다</param>
    public void SetCurrentSe(string SeName){
        if(!SoundEffectFileName.Contains(SeName)){
            Debug.LogError($"SetCurrentSe에서의 에러\n{SeName}이라는 Se 파일은 존재하지 않습니다.");
        }
        CurrentSeFile = SeName;
    }

    /// <summary>
    /// 현재 Bgm을 설정합니다. 
    /// </summary>
    /// <param name="BgmName">설정하길 원하는 bgm의 이름입니다.</param>
    public void SetCurrentBgm(string BgmName){
         if(!BgmAudioFileName.Contains(BgmName)){
            Debug.LogError($"SetCurrentBgm에서의 에러\n{BgmName}이라는 Bgm 파일은 존재하지 않습니다.");
        }
        CurrentBgmFile = BgmName;

    }
    /// <summary>
    /// 현재 Bgm을 설정합니다. 
    /// </summary>
    /// <param name="VoiceName">설정하길 원하는 bgm의 이름입니다.</param>
    public void SetCurrentVoice(string VoiceName){
         if(!VoiceAudioFileName.Contains(VoiceName)){
            Debug.LogError($"SetCurrentVoice에서의 에러\n{VoiceName}이라는 Voice 파일은 존재하지 않습니다.");
        }
        CurrentVoiceFile = VoiceName;

    }


    /*[출력 부분]*/

    /// <summary>
    /// 현재 효과음 파일을 출력합니다. 
    /// <para>
    /// 단 한 번만 출력합니다. 
    /// </para>
    /// </summary>
    public void PlayCurrentSe(){
        SeAudioSource.PlayOneShot(SoundEffect[CurrentSeFile]);
    }

    /// <summary>
    /// 현재 Bgm 파일을 출력합니다. 출력은 지속됩니다. 
    /// </summary>
    public void PlayCurrentBgm(){
        BgmAudioSource.clip = BgmAudio[CurrentBgmFile];
        BgmAudioSource.Play();

    }
    public void PlayCurrentVoice(){
        VoiceAudioSource.PlayOneShot(VoiceAudio[CurrentVoiceFile]);
    }

    /*[보이스 소스 조절 부분]*/

    /// <summary>
    /// 보이스의 피치를 조절합니다. -3 ~ 3까지만 조절 가능합니다.
    /// </summary>
    /// <param name="pitch">얼마나 조절할지 결정합니다.</param>
    public void SetVoicePitch(float pitch){
        VoiceAudioSource.pitch += pitch;
        VoiceAudioSource.pitch = Mathf.Clamp(VoiceAudioSource.pitch, -3f, 3f); // 🎯 피치 범위 제한 (-3 ~ 3)

    }

    /// <summary>
    /// 보이스의 볼륨을 조절합니다. 0 ~ 1까지만 조절 가능합니다.
    /// </summary>
    /// <param name="size"></param>

    public void SetVoiceVolum(float size){
        VoiceAudioSource.volume = Mathf.Clamp01(size);
    }
}

