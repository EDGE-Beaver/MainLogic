using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : FileManager
{
    [Header("UI 요소")]
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
        LoadAllTextFiles(); // 파일 로드
    }

    /// <summary>
    /// 특정 파일의 첫 번째 대화 시작
    /// </summary>
    /// <param name="fileName">대화를 시작할 파일 이름</param>
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
            Debug.LogError($"파일 '{fileName}'에 대화 데이터가 없습니다.");
        }
    }

    /// <summary>
    /// 현재 인덱스의 대화를 UI에 표시
    /// </summary>
    private void LoadDialogue()
    {
        string[] row = GetRowByIndex(currentFileName, currentIndex);
        if (row != null)
        {
            string speaker = row.Length > 0 ? row[0] : "알 수 없음";
            string dialogue = row.Length > 1 ? row[1] : "대화 내용 없음";
            string seFileName = row.Length > 2 ? row[2] : null;
            string imageFileName = row.Length > 3 ? row[3] : null;
            string scene = row.Length > 4 ? row[4] : null;
            string[] choices = row.Length > 5 ? row[5].Split(',') : null;

            // 화자 이름 설정
            speakerNameText.text = speaker;

            // 대화 내용 타이핑 애니메이션
            StartCoroutine(TypeDialogue(dialogue));

            // 이미지 설정
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

            // 사운드 효과 재생
            if (!string.IsNullOrEmpty(seFileName))
            {
                AudioClip se = Resources.Load<AudioClip>(seFileName);
                if (se != null)
                {
                    audioSource.PlayOneShot(se);
                }
            }

            // 선택지 처리
            if (choices != null && choices.Length > 0)
            {
                ShowChoices(new List<string>(choices));
            }
        }
        else
        {
            Debug.Log("대화가 종료되었습니다.");
        }
    }

    /// <summary>
    /// 선택지 UI 표시
    /// </summary>
    private void ShowChoices(List<string> choices)
    {
        // 기존 선택지 버튼 제거
        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();

        // 선택지 버튼 생성
        choicePanel.SetActive(true);
        for (int i = 0; i < choices.Count; i++)
        {
            var button = Instantiate(choiceButtonPrefab, choicePanel.transform);
            button.GetComponentInChildren<Text>().text = choices[i];
            int choiceIndex = i; // 클로저 문제 방지
            button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            choiceButtons.Add(button);
        }
    }

    /// <summary>
    /// 선택지가 선택되었을 때 호출
    /// </summary>
    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"선택한 선택지: {choiceIndex}");
        choicePanel.SetActive(false);

        // 선택지에 따른 인덱스 변경 (필요에 따라 확장 가능)
        currentIndex++;
        LoadDialogue();
    }

    /// <summary>
    /// 타이핑 애니메이션을 통해 대화 내용을 표시
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
            Debug.Log("마지막 대화입니다.");
        }
    }
}
