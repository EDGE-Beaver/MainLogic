using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImageManager : MonoBehaviour
{
    [Header("이 씬에서 필요한 캐릭터 이미지 파일 경로들")]
    public List<string> ImageFilePath = new List<string>();

    [Header("애니메이션 스크립트들")]

    [Tooltip("끄덕임 효과")]
    public NodEffect nodEffect;//끄덕임 효과
    public List<MonoBehaviour> scripts = new List<MonoBehaviour>();


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
