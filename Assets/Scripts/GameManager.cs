using System.Collections.Generic;
using UnityEngine;

public class GameManager : FileManager
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
        LoadAllTextFiles(); // ���� �ε�
        StartDialogue();
    }

    /// <summary>
    /// ��ȭ�� �����ϴ� �޼���
    /// </summary>
    public void StartDialogue()
    {
        currentIndex = 0; // ��ȭ ���� �� �ε��� �ʱ�ȭ
        if (GetRowCount(currentFileName) > 0)
        {
            LoadDialogue();
        }
        else
        {
            Debug.LogError($"���� '{currentFileName}'�� ��ȭ �����Ͱ� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� �ε����� ��ȭ�� UI�� ǥ��
    /// </summary>
    public void LoadDialogue()
    {
        string[] row = GetRowByIndex(currentFileName, currentIndex);
        if (row != null)
        {
            string speaker = row.Length > 0 ? row[0] : "�� �� ����";
            string dialogue = row.Length > 1 ? row[1] : "��ȭ ���� ����";
            string seFileName = row.Length > 2 ? row[2] : null;
            string imageFileName = row.Length > 3 ? row[3] : null;
            string sceneData = row.Length > 4 ? row[4] : null;
            string[] choices = row.Length > 5 ? row[5].Split(',') : null;

            // UI�� ��ȭ ǥ��
            dialogueUI.ShowDialogue(speaker, dialogue);

            // �������� �ִ� ���
            if (choices != null && choices.Length > 0)
            {
                choiceManager.ShowChoices(choices, OnChoiceSelected);
            }

            // ���� ȿ��, �̹���, �� ������ Ȱ�� ���� �߰� ����
            if (!string.IsNullOrEmpty(seFileName))
            {
                // SE ��� ���� �߰� ����
            }

            if (!string.IsNullOrEmpty(imageFileName))
            {
                // �̹��� ǥ�� ���� �߰� ����
            }
        }
        else
        {
            Debug.Log("�� �̻� ��ȭ�� �����ϴ�.");
        }
    }

    /// <summary>
    /// �������� ���õǾ��� �� ȣ��
    /// </summary>
    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"������ ������: {choiceIndex}");
        currentIndex += 1 + choiceIndex; // ���ÿ� ���� �ε��� �̵�
        LoadDialogue();
    }

    /// <summary>
    /// ���� ��ȭ�� �̵�
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
            Debug.Log("������ �����߽��ϴ�.");
            TriggerEnding();
        }
    }

    /// <summary>
    /// ���� ��ȭ�� �̵�
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
            Debug.Log("�������Դϴ�. ���� �������� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� ȣ��
    /// </summary>
    private void TriggerEnding()
    {
        Debug.Log("���� �Լ� ȣ��!");
        // ���� �б� ���� �߰� ����
    }
}
