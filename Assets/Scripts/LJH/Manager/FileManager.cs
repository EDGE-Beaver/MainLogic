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

    public string currentFile { get; private set; }

    private Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();

    void Awake()
    {
        LoadAllTextFiles();
    }

    public void LoadAllTextFiles()
    {
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
