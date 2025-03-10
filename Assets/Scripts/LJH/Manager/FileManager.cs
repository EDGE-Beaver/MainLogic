// ===============================================================
// FileManager.cs
// 제작자: 이지환
// 설명: 이 스크립트는 텍스트 파일을 로드하고 데이터를 관리하는 역할을 합니다.
//      다이얼로그 데이터를 다루지만, 다이얼로그 매니저가 실질적인 역할을 수행하며,
//      코드 길이가 너무 길어지는 것을 방지하기 위해 분할되었습니다.
//      퍼킹 싱글톤은 더 이상 존재하지 않습니다.
// 사용법:
//      1. Resources 폴더 내의 텍스트 파일을 로드하여 데이터를 관리합니다.
//      2. 특정 파일을 선택하여 데이터를 가져올 수 있습니다.
//      3. 인덱스를 기반으로 텍스트 데이터를 검색할 수 있습니다.
// ===============================================================

/* [변경사항 및 리뷰] (2/14, 박준건)
* 1. 함수 설명 추가
*
*
*/

//==============[함수 설명]===================
/*

 [Awake()]

 내용 - 인스펙터 창에 지정된 내용대로 파일을 읽어옵니다. 
 
 []

*/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [Header("텍스트 파일 이름 목록 (Resources 폴더 기준)")]
    public List<string> textFilePaths;

    [Header("텍스트 파일 구분자")]
    public char delimiter = '|';


    /// <summary>
    /// 현재 파일의 이름을 제공해줍니다. 
    /// <para>
    /// get - set 캡슐화가 적용되어 있습니다. 
    /// </para>
    /// </summary>
    public string currentFile { get; private set; }

    /// <summary>
    /// string : 파일 이름, List : 파일 저장 형태(스트링 형태로 저장)
    /// <para>
    /// 화자 이름|대사 내용|효과음|캐릭터 이미지|선택지ID|음성(캐릭터 목소리)|배경음악|애니메이션 키워드
    /// </para>
    /// <para>
    /// 이런 형태의 데이터들이 저장되어 있습니다
    /// </para>
    /// </summary>
    private Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();

    void Awake()
    {
        LoadAllTextFiles();
    }
    /// <summary>
    /// 이 씬에 할당된 텍스트 파일을 읽어옵니다. 
    /// </summary>
    public void LoadAllTextFiles()
    {
        /*============[작동 원리]==============

        0. 만일 로드된 데이터들이 있다면, 죄다 지워버린다. 
        1. 
        */
        loadedData.Clear();


        foreach (var filePath in textFilePaths)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"🚨 텍스트 파일을 찾을 수 없습니다: {filePath}");
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

            if (string.IsNullOrEmpty(currentFile))
            {
                currentFile = fileName;
            }

            Debug.Log($"📂 텍스트 파일 로드 완료: {fileName}");
        }
    }

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
        }
    }

    public string[] GetRowByIndex(string fileName, int index)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogWarning($"⚠️ 파일 '{fileName}'이(가) 로드되지 않았습니다.");
            return new string[] { "" };
        }

        var dataList = loadedData[fileName];

        if (index < 0 || index >= dataList.Count)
        {
            Debug.LogWarning($"⚠️ '{fileName}' 파일의 잘못된 인덱스 요청: {index}");
            return new string[] { "" };
        }

        return dataList[index];
    }
}
