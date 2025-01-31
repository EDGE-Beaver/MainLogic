using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;




public abstract class FileManager : MonoBehaviour
{

    /*
     * 
     * 화자 이름|대화 내용|SE 파일 이름|이미지 파일 이름|씬 데이터|선택지1,선택지2,선택지3
     * 예시:
    NPC1|안녕하세요! 여기는 테스트 씬입니다.|se_greeting|npc1_image|Scene1|
    NPC2|무엇을 도와드릴까요?|se_help|npc2_image|Scene2|도움 요청하기,그냥 떠난다

    수정사항(박준건)
    어차피 씬마다 텍스트파일 불러오는게 다르기 때문에 씬 데이터는 의미 없을 듯
    화자 이름|대화 내용|SE 파일 이름|이미지 파일 이름|선택지 관련 파일(0일시 없고, 1일시 1번 선택지를 제공)
     * 
     */


    /// <summary>
    /// 로드된 데이터를 저장하는 딕셔너리: 파일 이름을 키로, 각 줄을 파싱한 데이터 리스트를 값으로 저장
    /// 키는 파일 이름, 값은 string 배열 형태의 데이터.
    /// </summary>
    protected Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();

    [Header("텍스트 파일 이름 목록 (Resources 폴더 기준)")]
    public List<string> textFilePaths;

    [Header("데이터 구분자")]
    public char delimiter = '|';

    /// <summary>
    /// 모든 텍스트 파일 로드
    /// </summary>
    protected void LoadAllTextFiles()
    {
        /*
        --------------------------------------------
        텍스트 파일 읽어오는 함수

        1. String 배열로 저장되어 있는 filepath의 개수만큼 다음을 반복함
            1. 파일 경로에 있는 텍스트 파일을 가져옴
            2. 개행 문자를 기준으로 나누고
            3. datalist(향후 딕셔너리에 넣기 위함)을 만듬
            4. 만약 라인이 비어있지 않다면 토큰화하고

        */
        foreach (var filePath in textFilePaths)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"텍스트 파일을 찾을 수 없습니다: {filePath}");
                continue;
            }

            var lines = textAsset.text.Split('\n');
            var dataList = new List<string[]>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var tokens = line.Split(delimiter);
                    dataList.Add(tokens);
                
                }
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            loadedData.Add(fileName, dataList);//

            Debug.Log($"텍스트 파일 로드 완료: {fileName}");
            foreach(var type in dataList){
                foreach(var str in type)
                {
                     Debug.Log(str);
                }
               
            }
        }
    }

    //밑은 나중에 검토함(박준건)
    /// <summary>
    /// 특정 파일의 특정 인덱스 데이터 가져오기
    /// </summary>
    /// <param name="fileName">파일 이름</param>
    /// <param name="index">가져올 인덱스</param>
    /// <returns>데이터 배열 (null 반환 가능)</returns>
    public string[] GetRowByIndex(string fileName, int index)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogError($"파일 이름 '{fileName}'에 해당하는 데이터가 없습니다.");
            return null;
        }

        var dataList = loadedData[fileName];
        if (index >= 0 && index < dataList.Count)
            return dataList[index];

        Debug.LogWarning($"잘못된 인덱스 요청: {index} (파일: {fileName})");
        return null;
    }

    /// <summary>
    /// 특정 파일의 총 행 수 반환
    /// </summary>
    /// <param name="fileName">파일 이름</param>
    /// <returns>총 행 수</returns>
    public int GetRowCount(string fileName)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogWarning($"파일 이름 '{fileName}'에 해당하는 데이터가 없습니다.");
            return 0;
        }

        return loadedData[fileName].Count;
    }

    /// <summary>
    /// 로드된 파일 이름 목록 반환
    /// </summary>
    /// <returns>파일 이름 리스트</returns>
    public List<string> GetLoadedFileNames()
    {
        return new List<string>(loadedData.Keys);
    }
}
