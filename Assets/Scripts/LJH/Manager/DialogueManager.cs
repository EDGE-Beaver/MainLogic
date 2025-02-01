using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("파일 매니저 (Inspector에서 지정)")]
    public FileManager fileManager;

    [Header("끄덕 애니메이션 효과 (Inspector에서 지정)")]
    public NodEffect nodEffect; // 🎯 NodEffect 인스펙터에서 지정 가능하도록 변경

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

    void Start()
    {
        if (fileManager == null)
        {
            Debug.LogError("🚨 FileManager가 설정되지 않았습니다! 인스펙터에서 지정하세요.");
            return;
        }

        fileManager.LoadAllTextFiles();
        choicePanel.SetActive(false);

        if (!string.IsNullOrEmpty(fileManager.currentFile))
        {
            LoadDialogue(fileManager.currentFile);
        }
        else
        {
            Debug.LogWarning("⚠️ 현재 FileManager가 읽고 있는 파일이 없습니다!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (choicePanel.activeSelf) return;

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
        if (fileManager == null)
        {
            Debug.LogError("🚨 FileManager가 설정되지 않았습니다! 인스펙터에서 설정하세요.");
            return;
        }

        string[] data = fileManager.GetRowByIndex(fileManager.currentFile, currentIndex);

        if (data == null || data.Length == 0)
        {
            Debug.LogWarning($"⚠️ '{fileManager.currentFile}' 파일에서 {currentIndex}번째 줄을 찾을 수 없습니다.");
            return;
        }

        string speaker = data.Length > 0 ? data[0].Trim() : "";
        string dialogue = data.Length > 1 ? data[1].Trim() : "";
        string se = data.Length > 2 ? data[2].Trim() : "";
        string image = data.Length > 3 ? data[3].Trim() : "";
        string choiceIndex = data.Length > 4 ? data[4].Trim() : "";
        string voice = data.Length > 5 ? data[5].Trim() : "";
        string bgm = data.Length > 6 ? data[6].Trim() : "";
        string animationKeyword = data.Length > 7 ? data[7].Trim() : ""; // 🎯 애니메이션 키워드 감지

        // 🎯 화자 이름 설정
        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;

        // 🎯 Voice 오디오 파일 로드 및 텍스트 출력
        AudioClip voiceClip = !string.IsNullOrEmpty(voice) ? Resources.Load<AudioClip>($"Audio/Voice/{voice}") : null;
        textAnimationScript.SetText(dialogue, voiceClip);

        // 🎯 SE 로드 및 재생
        if (!string.IsNullOrEmpty(se))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/SE/{se}");
            if (clip != null)
            {
                seAudioSource.clip = clip;
                seAudioSource.Play();
            }
        }

        // 🎯 이미지 로드 및 설정
        if (!string.IsNullOrEmpty(image))
        {
            Sprite sprite = Resources.Load<Sprite>($"Graphics/Image/{image}");
            if (sprite != null)
            {
                characterImage.sprite = sprite;
            }
        }

        // 🎯 BGM 로드 및 재생
        if (!string.IsNullOrEmpty(bgm))
        {
            AudioClip bgmClip = Resources.Load<AudioClip>($"Audio/BGM/{bgm.Trim()}");
            if (bgmClip != null)
            {
                if (bgmAudioSource.clip != bgmClip)
                {
                    bgmAudioSource.clip = bgmClip;
                    bgmAudioSource.Play();
                }
            }
        }

        // 🎯 "끄덕" 키워드가 포함된 경우 애니메이션 실행
        if (!string.IsNullOrEmpty(animationKeyword) && animationKeyword == "끄덕")
        {
            if (nodEffect != null)
            {
                nodEffect.StartNod();
            }
            else
            {
                Debug.LogWarning("⚠️ NodEffect가 인스펙터에서 연결되지 않았습니다.");
            }
        }

        // 🎯 선택지 표시 처리
        if (!string.IsNullOrEmpty(choiceIndex))
        {
            StartCoroutine(ShowChoicePanel(choiceIndex));
        }

        currentIndex++;
    }

    IEnumerator ShowChoicePanel(string choiceIndex)
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
