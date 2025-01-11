using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : FileManager
{
    [Header("UI ���")]
    public Text speakerNameText;
    public Text dialogueText;
    public Image characterImage;
    public GameObject choicePanel;
    public Button choiceButtonPrefab;
    public AudioSource audioSource;

    public float typingSpeed = 0.05f;

    private List<Button> choiceButtons = new List<Button>();
    private int currentIndex = 0;
    private string currentFileName = "";

    void Start()
    {
        LoadAllTextFiles(); // ���� �ε�
    }

    /// <summary>
    /// Ư�� ������ ù ��° ��ȭ ����
    /// </summary>
    /// <param name="fileName">��ȭ�� ������ ���� �̸�</param>
    public void StartDialogue(string fileName)
    {
        currentFileName = fileName;
        currentIndex = 0;

        if (GetRowCount(fileName) > 0)
        {
            LoadDialogue();
        }
        else
        {
            Debug.LogError($"���� '{fileName}'�� ��ȭ �����Ͱ� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� �ε����� ��ȭ�� UI�� ǥ��
    /// </summary>
    private void LoadDialogue()
    {
        string[] row = GetRowByIndex(currentFileName, currentIndex);
        if (row != null)
        {
            string speaker = row.Length > 0 ? row[0] : "�� �� ����";
            string dialogue = row.Length > 1 ? row[1] : "��ȭ ���� ����";
            string seFileName = row.Length > 2 ? row[2] : null;
            string imageFileName = row.Length > 3 ? row[3] : null;
            string scene = row.Length > 4 ? row[4] : null;
            string[] choices = row.Length > 5 ? row[5].Split(',') : null;

            // ȭ�� �̸� ����
            speakerNameText.text = speaker;

            // ��ȭ ���� Ÿ���� �ִϸ��̼�
            StartCoroutine(TypeDialogue(dialogue));

            // �̹��� ����
            if (!string.IsNullOrEmpty(imageFileName))
            {
                Sprite image = Resources.Load<Sprite>(imageFileName);
                if (image != null)
                {
                    characterImage.sprite = image;
                    characterImage.gameObject.SetActive(true);
                }
                else
                {
                    characterImage.gameObject.SetActive(false);
                }
            }

            // ���� ȿ�� ���
            if (!string.IsNullOrEmpty(seFileName))
            {
                AudioClip se = Resources.Load<AudioClip>(seFileName);
                if (se != null)
                {
                    audioSource.PlayOneShot(se);
                }
            }

            // ������ ó��
            if (choices != null && choices.Length > 0)
            {
                ShowChoices(new List<string>(choices));
            }
        }
        else
        {
            Debug.Log("��ȭ�� ����Ǿ����ϴ�.");
        }
    }

    /// <summary>
    /// ������ UI ǥ��
    /// </summary>
    private void ShowChoices(List<string> choices)
    {
        // ���� ������ ��ư ����
        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();

        // ������ ��ư ����
        choicePanel.SetActive(true);
        for (int i = 0; i < choices.Count; i++)
        {
            var button = Instantiate(choiceButtonPrefab, choicePanel.transform);
            button.GetComponentInChildren<Text>().text = choices[i];
            int choiceIndex = i; // Ŭ���� ���� ����
            button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            choiceButtons.Add(button);
        }
    }

    /// <summary>
    /// �������� ���õǾ��� �� ȣ��
    /// </summary>
    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"������ ������: {choiceIndex}");
        choicePanel.SetActive(false);

        // �������� ���� �ε��� ���� (�ʿ信 ���� Ȯ�� ����)
        currentIndex++;
        LoadDialogue();
    }

    /// <summary>
    /// Ÿ���� �ִϸ��̼��� ���� ��ȭ ������ ǥ��
    /// </summary>
    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
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
            Debug.Log("������ ��ȭ�Դϴ�.");
        }
    }
}
