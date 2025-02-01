using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour  // 🎯 MonoBehaviour 상속 추가
{
    [Header("텍스트 파일 이름 목록 (Resources 폴더 기준)")]
    public List<string> textFilePaths;

    [Header("텍스트 파일 구분자")]
    public char delimiter = '|';

    /// <summary>
    /// 로드된 대화 데이터를 저장하는 딕셔너리
    /// 키: 파일 이름, 값: 각 줄을 파싱한 데이터 리스트 (각 줄은 문자열 배열)
    /// </summary>
    private Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();

    void Awake()
    {
        LoadAllTextFiles();
    }

    /// <summary>
    /// 모든 텍스트 파일을 로드하는 함수
    /// </summary>
    public void LoadAllTextFiles()
    {
        loadedData.Clear();

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
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var tokens = line.Split(delimiter);
                    dataList.Add(tokens);
                }
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            loadedData[fileName] = dataList;

            Debug.Log($"텍스트 파일 로드 완료: {fileName}");
        }
    }

    /// <summary>
    /// 특정 파일의 특정 줄을 가져오는 함수
    /// </summary>
    public string[] GetRowByIndex(string fileName, int index)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogWarning($"⚠️ 파일 '{fileName}'이(가) 로드되지 않았습니다.");
            return new string[] { "" }; // ⚠️ null 반환하지 않고 빈 배열 반환
        }

        var dataList = loadedData[fileName];

        if (index < 0 || index >= dataList.Count)
        {
            Debug.LogWarning($"⚠️ '{fileName}' 파일의 잘못된 인덱스 요청: {index}");
            return new string[] { "" }; // ⚠️ null 반환하지 않고 빈 배열 반환
        }

        return dataList[index];
    }
}
