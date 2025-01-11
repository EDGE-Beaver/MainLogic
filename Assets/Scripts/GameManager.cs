using System.Collections.Generic;
using UnityEngine;

public class GameManager : FileManager
{
    [Header("UI 관련 요소")]
    [Tooltip("DialogueUI 스크립트가 추가된 GameObject를 지정하세요.")]
    public DialogueUI dialogueUI;  // 인스펙터에서 지정 가능

    [Tooltip("ChoiceManager 스크립트가 추가된 GameObject를 지정하세요.")]
    public ChoiceManager choiceManager;  // 인스펙터에서 지정 가능

    private int currentIndex = 0;
    private string currentFileName = "DialogueFile1"; // 시작할 파일 이름

    void Start()
    {
        LoadAllTextFiles(); // 파일 로드
        StartDialogue();
    }

    /// <summary>
    /// 대화를 시작하는 메서드
    /// </summary>
    public void StartDialogue()
    {
        currentIndex = 0; // 대화 시작 시 인덱스 초기화
        if (GetRowCount(currentFileName) > 0)
        {
            LoadDialogue();
        }
        else
        {
            Debug.LogError($"파일 '{currentFileName}'에 대화 데이터가 없습니다.");
        }
    }

    /// <summary>
    /// 현재 인덱스의 대화를 UI에 표시
    /// </summary>
    public void LoadDialogue()
    {
        string[] row = GetRowByIndex(currentFileName, currentIndex);
        if (row != null)
        {
            string speaker = row.Length > 0 ? row[0] : "알 수 없음";
            string dialogue = row.Length > 1 ? row[1] : "대화 내용 없음";
            string seFileName = row.Length > 2 ? row[2] : null;
            string imageFileName = row.Length > 3 ? row[3] : null;
            string sceneData = row.Length > 4 ? row[4] : null;
            string[] choices = row.Length > 5 ? row[5].Split(',') : null;

            // UI에 대화 표시
            dialogueUI.ShowDialogue(speaker, dialogue);

            // 선택지가 있는 경우
            if (choices != null && choices.Length > 0)
            {
                choiceManager.ShowChoices(choices, OnChoiceSelected);
            }

            // 사운드 효과, 이미지, 씬 데이터 활용 로직 추가 가능
            if (!string.IsNullOrEmpty(seFileName))
            {
                // SE 재생 로직 추가 가능
            }

            if (!string.IsNullOrEmpty(imageFileName))
            {
                // 이미지 표시 로직 추가 가능
            }
        }
        else
        {
            Debug.Log("더 이상 대화가 없습니다.");
        }
    }

    /// <summary>
    /// 선택지가 선택되었을 때 호출
    /// </summary>
    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"선택한 선택지: {choiceIndex}");
        currentIndex += 1 + choiceIndex; // 선택에 따른 인덱스 이동
        LoadDialogue();
    }

    /// <summary>
    /// 다음 대화로 이동
    /// </summary>
    public void NextDialogue()
    {
        if (currentIndex < GetRowCount(currentFileName) - 1)
        {
            currentIndex++;
            LoadDialogue();
        }
        else
        {
            Debug.Log("엔딩에 도달했습니다.");
            TriggerEnding();
        }
    }

    /// <summary>
    /// 이전 대화로 이동
    /// </summary>
    public void PreviousDialogue()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            LoadDialogue();
        }
        else
        {
            Debug.Log("시작점입니다. 이전 페이지가 없습니다.");
        }
    }

    /// <summary>
    /// 엔딩 호출
    /// </summary>
    private void TriggerEnding()
    {
        Debug.Log("엔딩 함수 호출!");
        // 엔딩 분기 로직 추가 가능
    }
}
