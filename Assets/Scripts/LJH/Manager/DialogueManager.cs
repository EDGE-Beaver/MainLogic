using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


// ===========================================
// 📌 [대사 데이터 파일 구조 설명]
// ===========================================
// 각 줄은 하나의 대사를 의미하며, "|"로 구분된 여러 개의 필드를 포함합니다.
// 필드별 역할 및 데이터 형식 설명은 아래와 같습니다.
//
// [필드 1] 화자 이름
//   - 대사를 말하는 캐릭터의 이름을 지정합니다.
//   - 예: "유 이연"
//
// [필드 2] 대사 내용
//   - 캐릭터가 출력할 대사 문자열입니다.
//   - 대사 내에서 특정 태그를 사용하여 다양한 연출 효과를 적용할 수 있습니다.
//
//   🔹 특수 태그 설명
//   ---------------------------------------------------------
//   *N  : 볼륨 조절 (0~1 사이 값, 예: *0.5 볼륨 낮춤, *1 원래 볼륨)
//   #N  : 피치 조절 (-3~3 사이 값, 예: #0.03 피치 증가, #-0.03 피치 감소)
//   \N  : 출력 속도 변경 (예: \2 -> 속도 2배 빠르게)
//   $N  : 출력 대기 시간 (예: $0.5 -> 0.5초 후 다음 글자 출력)
//   ^   : 끄덕 애니메이션 실행
//   %   : 선택지 표시 (해당 줄이 선택지 패널을 띄우도록 처리)
//   ---------------------------------------------------------
//
// [필드 3] 효과음 (SE, Sound Effect)
//   - 특정 대사에서 재생할 효과음 파일의 이름을 지정합니다.
//   - 예: "Knock" (Knock.mp3 파일 재생)
//
// [필드 4] 캐릭터 이미지
//   - 대사 진행 중 변경할 캐릭터의 이미지 파일명을 지정합니다.
//   - 예: "image_nurse_concept_1"
//
// [필드 5] 선택지 ID
//   - 특정 대사에서 선택지가 나타날 경우 해당 선택지의 ID를 설정합니다.
//   - 없을 경우 빈 문자열("")로 설정합니다.
//   - 예: "1" (선택지가 있는 경우), "" (선택지가 없는 경우)
//
// [필드 6] 음성 (Voice)
//   - 대사에 맞춰 재생할 음성 파일명을 지정합니다.
//   - 예: "voice" (voice.mp3 파일 재생)
//
// [필드 7] 배경 음악 (BGM)
//   - 특정 대사에서 변경할 배경 음악을 지정합니다.
//   - 예: "downvoice"
//
// [필드 8] 애니메이션 키워드
//   - 특정 애니메이션을 실행하기 위한 키워드를 지정합니다.
//   - 예: "끄덕" (끄덕이는 애니메이션 실행), "어둠" (화면 어두워짐 등)
//
// ===========================================
// 🔹 [파일 예시]
// ===========================================
// 유 이연|지금 기분이 어떠세요?\1.5$0.5 어디 아픈 곳은\0.5 없나요?||image_nurse_concept_1||voice|간호사_기본|
// 유 이연|괜찮아요.$1 천천히 숨 쉬세요.\2.5$0.5 깊게*0 ...$0.5 *1천천히*0 ...||image_nurse_concept_1||downvoice||
// 유 이연|*1그럼,$0.5 오늘도 평범한 하루를 보내도록 해요.|Knock|image_nurse_concept_1||voice||끄덕|어둠
//
// ===========================================
// 🔹 [추가 사항]
// ===========================================
// - 대사 파일을 수정할 때, 태그 사용 시 주의해야 합니다.
// - 각 필드는 "|" 구분자로 나뉘며, 개수가 맞지 않을 경우 오류가 발생할 수 있습니다.
// - 태그를 올바르게 사용하여 연출을 풍부하게 만들 수 있습니다.
//
// ===========================================
// 📌 이 주석은 대사 데이터 파일을 쉽게 이해하고 관리할 수 있도록 작성되었습니다.
// ===========================================


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