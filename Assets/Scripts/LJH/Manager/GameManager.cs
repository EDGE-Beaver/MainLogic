using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI ���� ���")]
    [Tooltip("DialogueUI ��ũ��Ʈ�� �߰��� GameObject�� �����ϼ���.")]
    public DialogueUI dialogueUI;  // �ν����Ϳ��� ���� ����

    [Tooltip("ChoiceManager ��ũ��Ʈ�� �߰��� GameObject�� �����ϼ���.")]
    public ChoiceManager choiceManager;  // �ν����Ϳ��� ���� ����

    private int currentIndex = 0;
    private string currentFileName = "DialogueFile1"; // ������ ���� �̸�

    void Start()
    {
        // DialogueUI�� ChoiceManager�� �ν����Ϳ��� �����Ǿ����� Ȯ��
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI�� �ν����Ϳ��� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (choiceManager == null)
        {
            Debug.LogError("ChoiceManager�� �ν����Ϳ��� �������� �ʾҽ��ϴ�!");
            return;
        }

        StartDialogue(currentFileName);
    }

    /// <summary>
    /// ��ȭ�� �����ϴ� �޼���
    /// </summary>
    /// <param name="fileName">��ȭ�� ������ ���� �̸�</param>
    public void StartDialogue(string fileName)
    {
        currentFileName = fileName;
        currentIndex = 0;
        dialogueUI.StartDialogue(fileName);
    }

    /// <summary>
    /// ���� ��ȭ�� �̵�
    /// </summary>
    public void NextDialogue()
    {
        dialogueUI.NextDialogue();
    }
}
