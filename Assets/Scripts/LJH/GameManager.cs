using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
        // DialogueUI와 ChoiceManager가 인스펙터에서 지정되었는지 확인
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI가 인스펙터에서 지정되지 않았습니다!");
            return;
        }

        if (choiceManager == null)
        {
            Debug.LogError("ChoiceManager가 인스펙터에서 지정되지 않았습니다!");
            return;
        }

        StartDialogue(currentFileName);
    }

    /// <summary>
    /// 대화를 시작하는 메서드
    /// </summary>
    /// <param name="fileName">대화를 시작할 파일 이름</param>
    public void StartDialogue(string fileName)
    {
        currentFileName = fileName;
        currentIndex = 0;
        dialogueUI.StartDialogue(fileName);
    }

    /// <summary>
    /// 다음 대화로 이동
    /// </summary>
    public void NextDialogue()
    {
        dialogueUI.NextDialogue();
    }
}
