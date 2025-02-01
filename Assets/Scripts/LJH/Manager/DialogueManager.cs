using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("파일 매니저 (Inspector에서 지정)")]
    public FileManager fileManager;

    [Header("끄덕 애니메이션 효과 (Inspector에서 지정)")]
    public NodEffect nodEffect;

    [Header("UI 요소들")]
    public TMP_Text speakerText;
    public TextAnimationScripts textAnimationScript;
    public Image characterImage;

    [Header("오디오 소스")]
    public AudioSource seAudioSource;
    public AudioSource voiceAudioSource;
    public AudioSource bgmAudioSource;

    [Header("SE (효과음) 기본값")]
    public AudioClip defaultSE;

    [Header("선택지 패널")]
    public GameObject choicePanel;
    public Button[] choiceButtons;

    private int currentIndex = 0;
    private bool isChoicePanelActive = false; // 선택지 패널 활성화 여부
    private bool isWaitingForText = false;   // 대사 출력이 끝날 때까지 키 입력 방지

    void Start()
    {
        if (fileManager == null)
        {
            Debug.LogError("FileManager가 설정되지 않았습니다!");
            return;
        }

        fileManager.LoadAllTextFiles();
        choicePanel.SetActive(false);
        isChoicePanelActive = false;

        foreach (var button in choiceButtons)
        {
            int index = System.Array.IndexOf(choiceButtons, button);
            button.onClick.AddListener(() => SelectChoice(index));
        }

        if (!string.IsNullOrEmpty(fileManager.currentFile))
            LoadDialogue(fileManager.currentFile);
    }

    void Update()
    {
        if (isChoicePanelActive || isWaitingForText) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (textAnimationScript.IsTyping)
            {
                textAnimationScript.SkipTyping();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void LoadDialogue(string fileName)
    {
        fileManager.SetCurrentFile(fileName);
        currentIndex = 0;
        ShowNextLine();
    }

    void ShowNextLine()
    {
        var data = fileManager.GetRowByIndex(fileManager.currentFile, currentIndex);
        if (data == null || data.Length == 0) return;

        string speaker = data[0].Trim();
        string dialogue = data[1].Trim();
        string se = data[2].Trim();
        string image = data[3].Trim();
        string choiceIndex = data[4].Trim();
        string voice = data[5].Trim();
        string bgm = data[6].Trim();
        string animationKeyword = data.Length > 7 ? data[7].Trim() : "";

        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;

        var voiceClip = !string.IsNullOrEmpty(voice) ? Resources.Load<AudioClip>($"Audio/Voice/{voice}") : null;

        // 대사를 출력하고 대사가 끝난 뒤 처리
        isWaitingForText = true; // 키 입력 방지
        textAnimationScript.SetText(dialogue, voiceClip,
            () =>
            {
               

                // 선택지가 있는 경우 선택지 패널 활성화
                if (!string.IsNullOrEmpty(choiceIndex))
                {
                    StartCoroutine(ShowChoicePanel());
                

                }
                
            },
            () =>
            {
                if (nodEffect != null) nodEffect.StartNod();
            });

        // SE 재생
        if (!string.IsNullOrEmpty(se))
            seAudioSource.PlayOneShot(Resources.Load<AudioClip>($"Audio/SE/{se}"));

        // 이미지 설정
        if (!string.IsNullOrEmpty(image))
            characterImage.sprite = Resources.Load<Sprite>($"Graphics/Image/{image}");

        // BGM 재생
        if (!string.IsNullOrEmpty(bgm))
        {
            var bgmClip = Resources.Load<AudioClip>($"Audio/BGM/{bgm}");
            if (bgmClip != bgmAudioSource.clip)
            {
                bgmAudioSource.clip = bgmClip;
                bgmAudioSource.Play();
            }
        }

        // 애니메이션 처리
        if (!string.IsNullOrEmpty(animationKeyword) && animationKeyword == "끄덕")
        {
            if (nodEffect != null)
                nodEffect.StartNod();
        }

        // 선택지가 없을 경우 바로 다음 대사로 이동
        if (string.IsNullOrEmpty(choiceIndex))
        {
            isWaitingForText = false;
            currentIndex++;
        }
    }

    IEnumerator ShowChoicePanel()
    {
        yield return new WaitForSeconds(0.5f);
        choicePanel.SetActive(true);
        isChoicePanelActive = true;
    }

    public void SelectChoice(int choiceIndex)
    {
        choicePanel.SetActive(false);
        isChoicePanelActive = false;
        currentIndex++;
        ShowNextLine();
    }
}