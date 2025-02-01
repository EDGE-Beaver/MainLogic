using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("파일 매니저 (Inspector에서 지정)")]
    public FileManager fileManager;  // 🎯 인스펙터에서 지정 가능하도록 변경

    [Header("UI 요소들")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image characterImage;
    public AudioSource audioSource;

    [Header("SE (효과음) 기본값")]
    public AudioClip defaultSE;

    [Header("선택지 패널")]
    public GameObject choicePanel;
    public Button[] choiceButtons;

    [Header("현재 사용 중인 파일")]
    public string currentFile;

    private int currentIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        if (fileManager == null)
        {
            Debug.LogError("🚨 FileManager가 설정되지 않았습니다! 인스펙터에서 지정하세요.");
            return;
        }

        fileManager.LoadAllTextFiles();
        choicePanel.SetActive(false);

        if (!string.IsNullOrEmpty(currentFile))
        {
            LoadDialogue(currentFile);
        }
        else
        {
            Debug.LogWarning("⚠️ currentFile이 설정되지 않았습니다!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (choicePanel.activeSelf) return;

            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void LoadDialogue(string fileName)
    {
        currentFile = fileName;
        currentIndex = 0;
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (fileManager == null)
        {
            Debug.LogError("🚨 FileManager가 설정되지 않았습니다! 인스펙터에서 설정하세요.");
            return;
        }

        string[] data = fileManager.GetRowByIndex(currentFile, currentIndex);

        if (data == null || data.Length == 0)
        {
            Debug.LogWarning($"⚠️ '{currentFile}' 파일에서 {currentIndex}번째 줄을 찾을 수 없습니다.");
            return;
        }

        string speaker = data.Length > 0 ? data[0].Trim() : "";
        string dialogue = data.Length > 1 ? data[1].Trim() : "";
        string se = data.Length > 2 ? data[2].Trim() : "";
        string image = data.Length > 3 ? data[3].Trim() : "";
        string choiceIndex = data.Length > 4 ? data[4].Trim() : "";

        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;
        StartCoroutine(TypeText(string.IsNullOrEmpty(dialogue) ? "..." : dialogue));

        if (!string.IsNullOrEmpty(se))
        {
            AudioClip clip = Resources.Load<AudioClip>(se);
            audioSource.PlayOneShot(clip != null ? clip : defaultSE);
        }

        if (!string.IsNullOrEmpty(image))
        {
            Sprite sprite = Resources.Load<Sprite>(image);
            if (sprite != null) characterImage.sprite = sprite;
        }

        currentIndex++;

        if (!string.IsNullOrEmpty(choiceIndex))
        {
            StartCoroutine(ShowChoicePanel());
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;
        dialogueText.text = fileManager.GetRowByIndex(currentFile, currentIndex - 1)[1];
    }

    IEnumerator ShowChoicePanel()
    {
        yield return new WaitForSeconds(0.5f);
        choicePanel.SetActive(true);
    }

    public void SelectChoice(int choiceIndex)
    {
        choicePanel.SetActive(false);
        ShowNextLine();
    }
}
