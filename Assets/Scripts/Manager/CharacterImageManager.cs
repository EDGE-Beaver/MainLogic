using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImageManager : MonoBehaviour
{
    [Header("이 씬에서 필요한 캐릭터 이미지 파일 경로들")]
    public List<string> ImageFilePath = new List<string>();
    private List<Sprite> ImageFile = new List<Sprite>();
    
    [Header("캐릭터 이미지 오브젝트들")] 
    public List<GameObject> ImageObj = new List<GameObject>();

    [Header("애니메이션 스크립트들")]

    [Tooltip("끄덕임 효과")]
    public NodEffect nodEffect;//끄덕임 효과
    public List<MonoBehaviour> scripts = new List<MonoBehaviour>();

    void Awake()
    {
        LoadAllImageFile();
    }

    private void LoadAllImageFile()
    {

    }

    private void playNodEffect()
    {
        //끄덕이는 애니메이션 실행
    }
    public void showCharcterImage(int IdentifyBinary, string CharacterName){


    }
     public void playCharacterAnimation(int IdentifyBinary, string AnimationName){
        switch(AnimationName){
            case "nod": 
                playNodEffect(); 
                break;
        }

    }

    //애니메이션 실행(캐릭터, 애니메이션)
    //캐릭터 배치(캐릭터 개수, 바이너리 코드)
    //

    //
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
