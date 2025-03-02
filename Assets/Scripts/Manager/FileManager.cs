// ===============================================================
// FileManager.cs
// 제작자: 이지환
// 설명: 이 스크립트는 텍스트 파일을 로드하고 데이터를 관리하는 역할을 합니다.
//      다이얼로그 데이터를 다루지만, 다이얼로그 매니저가 실질적인 역할을 수행하며,
//      코드 길이가 너무 길어지는 것을 방지하기 위해 분할되었습니다.
//      퍼킹 싱글톤은 더 이상 존재하지 않습니다.
//
//      파일 매니저가 파일을 가지고 있기 때문에, 일일히 파일을 불러올 때마다 객체를 생성하는 짓을 하지 않아도 됩니다. 
// 사용법:
//      1. Resources 폴더 내의 텍스트 파일을 로드하여 데이터를 관리합니다.
//      2. 특정 파일을 선택하여 데이터를 가져올 수 있습니다.
//      3. 인덱스를 기반으로 텍스트 데이터를 검색할 수 있습니다.
//
// 구조도
// 1. File을 읽어옴 
// 2. Setcurrentfile로 외부에서 어떤 file을 읽어올지 지정
// 3. Currentfile에서
//    3.1 GetRowbyindex = 인덱스에 따라 줄을 읽어옴
//    3.2 GetCurrentFileSize = 파일의 사이즈를 읽어옴. 
// ===============================================================

/* [변경사항 및 리뷰] (2/14, 박준건)
* 1. 함수 설명 추가
* 2. Dialog 파일과 선택지 파일을 전부 가지고 있게 변경. 
* 3. GetAllDialogFileNameItHave() 추가
* 4. 다른 기능들 추가
* 5. currentFIle을 중심으로 동작하도록 로직 변경
* 6. 오류 점검 및 테스트(통과)
*/

//==============[함수 설명]===================
/*

 * [Awake()]

 인스펙터 창에 지정된 내용대로 파일을 읽어옵니다. 
 
 * [LoadedAllTextFile] / [LoadedAllChoiceFile]

 - 지정된 파일들의 내부 텍스트 값을 읽어와서, 딕셔너리에 저장합니다. 
   이후 파일 이름을 키로 접근해서, 파일 내부 값들을 확인할 수 있습니다. 

 - 선택지 파일과 텍스트 파일을 전부 읽어서 저장해둡니다. 


 * [SetCurrentFile(string fileName)]

   - CurrentFile을 지정합니다. 

   이후 getRowIndex나, GetCurrentFileSize등의 모든 메소드는
   이 함수를 통해 지정된 currentfile에서 동작합니다. 
   만일 없다면 오류를 리턴합니다. 


 * [GetRowByIndex(int index)]

   - CurrentFIle에서, 매개변수로 받은 index에 위치하는 string[]을 리턴합니다. 

  유 이연|지금 기분이 어떠세요?\1.5$0.5 어디 아픈 곳은\0.5 없나요?||image_nurse_concept_1||voice|간호사_기본|
  이런 문장을 리턴합니다. 


 * [GetCurrentFileLength()]

 - CurrentFile의 길이를 리턴합니다. 
   null을 가질 수 있습니다. 

 * [GetAllDialogFileNameItHave()]
 
 현재 이 씬에서 Filemanager가 가지고 있는 모든 텍스트 파일 이름을 리턴합니다. 


 (테스트 여부)
 2/15 정상작동 확인
 ============================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [Header("텍스트 파일 경로 목록 (Resources 폴더 기준)")]
    public List<string> textFilePaths;
    public List<string> choiceFilePaths;

    [Header("텍스트 파일 구분자")]
    public char delimiter = '|';


    /// <summary>
    /// 현재 읽고 있는 파일입니다.
    /// <para>
    /// get - set 캡슐화가 적용되어 있습니다. 
    /// </para>
    /// </summary>
    public string currentFile { get; private set; }

    /// <summary>
    /// 현재 \읽고 있는 선택지 파일입니다.  
    /// <para>
    /// get - set 캡슐화가 적용되어 있습니다. 
    /// </para>
    /// </summary
    public string currentChoiceFile{get; private set;}


    /// <summary>
    /// string : 파일 이름, List : 파일 저장 형태(스트링 형태로 저장)
    /// <para>
    /// 화자 이름|대사 내용|효과음|캐릭터 이미지|선택지ID|음성(캐릭터 목소리)|배경음악|애니메이션 키워드
    /// </para>
    /// <para>
    /// 이런 형태의 데이터 한 줄 한 줄이 string[]로, 전체는 list로 저장되어 저장되어 있습니다
    /// </para>
    /// </summary>
    private Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();
    /// <summary>
    /// 중복 검사 / 존재 검사용 세트
    /// </summary>
    public HashSet<string> TextFileNameSet = new HashSet<string>();
    /// <summary>
    /// string : 파일 이름, List : 파일 저장 형태(스트링 형태로 저장)
    /// <para>
    ///  * 인덱스 | 선택지 내용 | 선택지에 따른 변수 변경 | 다음으로 읽을 파일(분기 결과) | 다음으로 읽을 파일의 인덱스
    /// </para>
    /// <para>
    /// 이런 형태의 데이터들이 저장되어 있습니다
    /// </para>
    /// </summary>
    private Dictionary<string, List<string[]>> loadedChoiceData = new Dictionary<string, List<string[]>>();
    /// <summary>
    /// 중복 검사용 / 이름 검사용 세트. 
    /// </summary>
    public HashSet<string> choiceFileNameSet = new HashSet<string>();

    [Header("테크니컬한 변수 저장되는 곳 - 테스트 끝나면 다 private로 변경")]
    [Tooltip("")]
    public bool isTextFileEnd = false;
    public bool isChoiceFileEnd = false;
    void Awake()
    {
        //텍스트 파일 읽어오기
        LoadAllTextFiles();

        //선택지 파일 읽어오기
        LoadAllChoiceFiles();
    }

    void Start()
    {
    }
    /// <summary>
    /// 이 씬에 할당된 텍스트 파일을 읽어옵니다. 
    /// </summary>
    public void LoadAllTextFiles()
    {
        loadedData.Clear();

        //읽어야 하는 파일들마다 반복
        foreach (var filePath in textFilePaths)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"🚨 텍스트 파일을 찾을 수 없습니다, LoadAllTextFiles에서의 에러 : {filePath}");
                continue;
            }

            var lines = textAsset.text.Split('\n');
            var dataList = new List<string[]>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var tokens = line.Split(delimiter);
                    dataList.Add(tokens);
                }
            }
            
            string fileName = Path.GetFileNameWithoutExtension(filePath); 

            loadedData[fileName] = dataList;
            TextFileNameSet.Add(fileName);//해시셋에 저장

            
            // if (string.IsNullOrEmpty(currentFile))
            // {
            //     currentFile = filePath;
            // }
            //currentFile은 SetCurrentfile에서 대체 가능하다고 생각했기 때문에 지움. 

            Debug.Log($"📂 텍스트 파일 로드 완료: {fileName}");
        }
    }
     public void LoadAllChoiceFiles()
    {
        loadedChoiceData.Clear();//파일 클리어

        //읽어야 하는 파일들마다 반복
        foreach (var filePath in choiceFilePaths)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"🚨 선택지 파일을 찾을 수 없습니다, LoadAllChoiceFiles에서의 에러 : {filePath}");
                continue;
            }

            var lines = textAsset.text.Split('\n');
            var dataList = new List<string[]>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var tokens = line.Split(delimiter);
                    dataList.Add(tokens);
                }
            }
            
            string fileName = Path.GetFileNameWithoutExtension(filePath); 

            loadedChoiceData[fileName] = dataList;
            choiceFileNameSet.Add(fileName);

            Debug.Log($"📂 선택지 파일 로드 완료: {fileName}");
       }
    }

    /// <summary>
    /// FileManager가 어떤 파일을 CurrentFile로 지정할지 정합니다.
    /// <para>
    /// 만약 그런 파일 존재하지 않을 경우 예외를 던집니다
    /// </para>
    /// </summary>
    /// <param name="fileName">파일의 이름입니다.</param>
    public void SetCurrentFile(string fileName)
    {
        if (loadedData.ContainsKey(fileName))
        {
            currentFile = fileName;
            Debug.Log($"📂 현재 파일 변경됨: {currentFile}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 파일 '{fileName}'이(가) 로드되지 않아 변경할 수 없습니다.");
            throw new FileDoesNotExistError("Filemanager-SetCurrentFile", "FIlemanager에 입력하지 않은 파일을 접근");
        }
    }
    /// <summary>
    /// FileManager가 어떤 파일을 CurrentchoiceFile로 지정할지 정합니다.
    /// <para>
    /// 만약 그런 파일 존재하지 않을 경우 예외를 던집니다
    /// </para>
    /// </summary>
    /// <param name="fileName">파일의 이름입니다.</param>
    public void SetCurrentChoiceFile(string fileName)
    {
        Debug.Log("초이스 매니저에서 여기까지 ㄴ들어옴");
        if (loadedChoiceData.ContainsKey(fileName))
        {
            currentChoiceFile = fileName;
            Debug.Log($"📂 현재 파일 변경됨: {currentChoiceFile}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 파일 '{fileName}'이(가) 로드되지 않아 변경할 수 없습니다.");
            throw new FileDoesNotExistError("Filemanager-SetCurrentChoiceFile", "FIlemanager에 입력하지 않은 파일을 접근");
        }
    }

    /// <summary>
    /// 다이얼로그 텍스트 규칙에 맞는 텍스트를 얻어오는 함수입니다.
    /// <para>
    /// CurrentFIle에서 데이터를 리턴합니다. 만일 변경을 원하신다면 SetCurrentfile 하십시오.
    /// </para>
    /// </summary>
    /// <param name="index">몇번 인덱스의 정보를 리턴할지 정합니다.</param>
    /// <returns>화자 | 데이터 | 등등.. 으로 이루어진 string 배열을 리턴합니다. </returns>
    public string[] GetRowByIndex(int index)
    {
        // if (!loadedData.ContainsKey(currentFile))
        // {
        //     Debug.LogWarning($"⚠️ 파일 '{currentFile}'이(가) 로드되지 않았습니다.");
        //     return new string[] { "" };
        // }
        //currentfile에서 읽어오도록 변경했기 때문에 확인 절차가 필요 없음
        if(currentFile == null){
            Debug.LogError("GetRowByIndex에서의 오류");
            Debug.LogError("CurrentFile이 존재하지 않습니다.");
        }
        var dataList = loadedData[currentFile];
        if (index < 0 || index >= dataList.Count)
        {
            Debug.LogWarning($"⚠️ '{currentFile}' 파일의 잘못된 인덱스 요청: {index}"); 
            if(index == dataList.Count){
                isTextFileEnd = true; //끝났다는 것을 알려준다. 
            }
            return new string[] { "" };
        }
        return dataList[index];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>

    public string[] GetChoiceRowByIndex(int index)
    {
        
        if(currentChoiceFile == null){
            Debug.LogError("GetChoiceRowByIndex에서의 오류");
            Debug.LogError("CurrentChoiceFile이 존재하지 않습니다.");
        }
        var dataList = loadedChoiceData[currentChoiceFile];
        if (index < 0 || index >= dataList.Count)
        {
            Debug.LogWarning($"⚠️ '{currentChoiceFile}' 파일의 잘못된 인덱스 요청: {index}");
            return new string[] { "" };
        }
        return dataList[index];
    }

    /// <summary>
    /// currentFile의 길이를 리턴합니다. null을 가질 수 있습니다.
    /// </summary>
    /// <returns>currentfile의 길이</returns>
    public int? GetCurrentFileLength(){
        if(currentFile != null ){
            return loadedData[currentFile].Count;
        }else{
            Debug.LogError("GetCurrentFileLength에서 발생한 오류"); 
            Debug.LogError("CurrentFile이 존재하지 않습니다");
            return null;
        }
        //테스트 완료.
    }
    public int? GetCurrentChoiceFileLength(){
        if(currentFile != null ){
            return loadedChoiceData[currentChoiceFile].Count;
        }else{
            Debug.LogError("GetCurrentChoiceFileLength에서 발생한 오류"); 
            Debug.LogError("CurrentChoiceFile이 존재하지 않습니다");
            return null;
        }
    }

    public string GetCurrentFileName(){
        if(currentFile != null){
            return currentFile;
        }else{
            Debug.LogError(" GetCurrentFileName에서의 오류");
            Debug.LogError("CurrentFile이 설정되지 않았습니다");
            return null;
        }

    }

    public string GetcurrentChoiceFileName(){
        if(currentChoiceFile != null){
            return currentChoiceFile;
        }else{
            Debug.LogError(" GetCurrentChoiceFileName에서의 오류");
            Debug.LogError("CurrentChoiceFile이 설정되지 않았습니다");
            return null;
        }

    }
    /// <summary>
    /// 현재 씬에서 이 FilaManager가 가지고 있는 모든 텍스트 파일 이름을 리턴합니다. 
    /// </summary>
    /// <returns>이 FileManage가 가지고 있는 모든 파일들의 이름이 담긴 String 배열입니다.</returns>
    public string[] GetAllDialogFileNameItHave(){
        var thisTextFile = loadedData.Keys.ToArray();
        return thisTextFile;
    }
    /// <summary>
    /// 현재 씬에서 이 FilaManager가 가지고 있는 모든 선택지 파일 이름을 리턴합니다. 
    /// </summary>
    /// 
     public string[] GetAllChoiceFileNameItHave(){
        var thisChoiceFile = loadedChoiceData.Keys.ToArray();
        return thisChoiceFile;
    }
}
