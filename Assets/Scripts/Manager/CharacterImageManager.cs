using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

//위치값 저장용
/*
middle = x : 3.05, y : 33
left1 = -690
left2 = -227
right1 = 234
right2 = 696
*/
public class CharacterImageManager : MonoBehaviour
{
    [Header("이 씬에서 필요한 캐릭터 이미지 파일 경로들")]
    public List<string> ImageFilePath = new List<string>();
    private Dictionary<string, Sprite> ImageFile = new Dictionary<string, Sprite>();
    public HashSet<string> ImageFileNameSet = new HashSet<string>();//중복검사 위한 셋
    
    [Header("캐릭터 이미지 오브젝트들")] 
    public List<GameObject> ImageObj = new List<GameObject>();
    /*
    0번 : 가운데 포지션
    1번 : 가장 왼쪽 포지션
    2번 : 좀 왼쪽 포지션
    3번 : 좀 오른쪽 포지션
    4번 : 매우 오른쪽 포지션

    대충 정렬해보면
    [xx0bx] <- 미들 포지션은 하나만 출력 가능.
    [1 2 3 4] <- 나머지는 대충 화면에 이렇게 배치됨.
    */

    [Header("캐릭터 위치값 저장하고 있는 변수들")]
    public HashSet<string> characterPosSet = new HashSet<string>();

    [Header("애니메이션 스크립트들")]
    //여기를 구현해야 함.

    [Tooltip("끄덕임 효과")]
    public NodEffect nodEffect;//끄덕임 효과

    [Header("테크니컬한 변수들, 테스트 끝나면 다 꺼놓을 것")]
    [Tooltip("위치값을 가지고 있음")]
    public Dictionary<string, Vector2> pos = new Dictionary<string, Vector2>(){
       {"Middle", new Vector2(3.05f, 33)},
       {"left1", new Vector2(-690, 33)},
       {"left2", new Vector2(-227, 33)},
       {"right1", new Vector2(234, 33)},
       {"right2", new Vector2(696, 33)}
    };

    void Awake()
    {
        LoadAllImageFile();
    }

    private void LoadAllImageFile()
    {
        //같은 캐릭터끼린 같은 배열로 묶이는게 어떨까 싶긴 했지만 그건 보류. 시간 없음.
       
        ImageFileNameSet.Clear();
        ImageFile.Clear();
        //읽어야 하는 파일들마다 반복
        foreach (var filePath in ImageFilePath)
        {
            Sprite imageAsset = Resources.Load<Sprite>(filePath);
            if (imageAsset == null)
            {
                Debug.LogError($"🚨 이미지 파일을 찾을 수 없습니다 : {filePath}");
                continue;
            }

    
            
            string fileName = Path.GetFileNameWithoutExtension(filePath); 

            ImageFile[fileName] = imageAsset;
            ImageFileNameSet.Add(fileName);

            Debug.Log($"📂 이미지 파일 로드 완료: {fileName}");
       }


    }
    /// <summary>
    /// String 타입의 캐릭터 이름을 받고, int의 position을 이용해 위치를 조절합니다.
    /// <para>
    /// position의 예시 : 0b1000 -> 왼쪽 첫번째 자리 / 0b0100 -> 왼쪽 두번째 자리 등등..
    /// </para>
    /// <para>
    /// 중앙에 캐릭터 배치할거면 나머지 영역엔 써놓지 마십시오
    /// </para>
    /// </summary>
    /// <param name="CharacterName">캐릭터의 이름입니다</param>
    /// <param name="position"></param>
    public void showCharcterImage(string CharacterName, int position){
        switch(position){
            case 0b0000:
                Image image = ImageObj[0].GetComponent<Image>();
                image.sprite = ImageFile[CharacterName];
                ImageObj[0].SetActive(true);
                break;
            case 0b1000:
                Image image1 = ImageObj[1].GetComponent<Image>();
                image1.sprite = ImageFile[CharacterName];
                ImageObj[1].SetActive(true);
                break;
            case 0b0100:
                Image image2 = ImageObj[2].GetComponent<Image>();
                image2.sprite = ImageFile[CharacterName];
                ImageObj[2].SetActive(true);
                break;
            case 0b0010:
                Image image3 = ImageObj[3].GetComponent<Image>();
                image3.sprite = ImageFile[CharacterName];
                ImageObj[3].SetActive(true);
                break;
            case 0b0001:
                Image image4 = ImageObj[4].GetComponent<Image>();
                image4.sprite = ImageFile[CharacterName];
                ImageObj[4].SetActive(true);
                break;
            default:
                Debug.LogError("제대로 된 값을 입력해주십쇼");
                break;
                
            
                
        }

    }
    public void SetAllImageFalse(){
         foreach(var obj in ImageObj){
            obj.SetActive(false);
        }
    }
}
