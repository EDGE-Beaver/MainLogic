
// ===========================================
// 📌 [대사 파일 구조 설명]
// ===========================================
// 각 줄은 하나의 대사를 의미하며, "|"로 구분된 여러 개의 필드를 포함함
// 필드별 역할 및 데이터 형식 설명은 아래와 같음
//
//
// 대사 파일 구조
//화자 이름|대사 내용|효과음|캐릭터 이미지|선택지ID|음성(캐릭터 목소리)|배경음악|애니메이션 키워드
//
// 배경 이미지 변경은 VariableManager에서 처리(이유는 스크립트 볼륨, 배경 이미지 사용 빈도 낮음)
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
//   @   : 맨 마지막에 등장할 경우 바로 다음 문장을 출력
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
//  아래 스크립트는 대사 파일을 관리하는 다이얼로그 매니저입니다.
//  파일 매니저, 텍스트애니메이션스크립트, 선택지매니저와 상속관계이므로 함부로 수정하지 말 것
// ===========================================
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

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

    [Header("선택지 매니저 (Inspector에서 지정)")]
    public ChoiceManager choiceManager; //  선택지 매니저 연결
    public FileManager filea = new FileManager();

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
        if (isChoicePanelActive) return; // 선택지 패널이 활성화된 경우 키 입력 차단

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (textAnimationScript.IsTyping) // 텍스트 애니메이션 중이면 스킵
            {
                textAnimationScript.SkipTyping();
            }
            else if (!isWaitingForText) // 대사 출력이 끝났다면 다음 대사로 이동
            {
                currentIndex++;
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

    public void ShowNextLineAfterChoice()
    {
        currentIndex++; // 🔹 선택지 이후 다음 인덱스로 이동
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (isChoicePanelActive) return; // 선택지 패널이 활성화되었을 때 키 입력 차단

        // 🔹 현재 데이터 가져오기
        var data = fileManager.GetRowByIndex(fileManager.currentFile, currentIndex);

        if (data == null || data.Length == 0)
        {
            Debug.Log($" 대사 파일의 마지막 줄에 도달했습니다. (currentIndex: {currentIndex})");
            return;
        }

        Debug.Log($" ShowNextLine 호출 (currentIndex: {currentIndex})");

        // 🔹 데이터 필드 분리
        string speaker = data[0]?.Trim();
        string dialogue = data[1]?.Trim();
        string se = data[2]?.Trim();
        string image = data[3]?.Trim();
        string choiceField = data[4]?.Trim();  // 선택지 파일명:ID (공백이면 선택지 없음)
        string voice = data[5]?.Trim();
        string bgm = data[6]?.Trim();
        string animationKeyword = data.Length > 7 ? data[7]?.Trim() : "";

        Debug.Log($" 대사 정보 - 화자: {speaker}, 대사: {dialogue}, 선택지 데이터: {choiceField}");

        // 🔹 `@` 태그 확인 → 즉시 다음 대사 출력
        if (dialogue.Contains("@"))
        {
            dialogue = dialogue.Replace("@", ""); // `@` 태그 제거
            speakerText.text = speaker;
            textAnimationScript.SetText(dialogue, null, () =>
            {
                Debug.Log(" `@` 태그 감지 → 자동으로 다음 대사 출력");
                currentIndex++;
                ShowNextLine(); // 다음 대사를 자동 실행
            }, null); // 🔹 `onTrigger` 매개변수에 `null` 추가

            return;
        }

        // 🔹 UI 텍스트 설정
        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;
        // 🔹 ^ 기호 제거
        if (dialogue.Contains("^"))
        {
            dialogue = dialogue.Replace("^", ""); // ^ 태그 제거
            Debug.Log(" 끄덕 태그(^): 제거됨");
        }
        // 🔹 % 태그 제거 및 선택지 여부 확인
        bool hasChoice = dialogue.Contains("%");
        if (hasChoice)
        {
            dialogue = dialogue.Replace("%", ""); // % 태그 제거
            Debug.Log(" 선택지 태그(%): 선택지 있음");
        }

        // 🔹 음성 클립 로드
        var voiceClip = !string.IsNullOrEmpty(voice)
            ? Resources.Load<AudioClip>($"Audio/Voice/{voice}")
            : null;

        // 🔹 선택지 관련 변수 초기화
        string choiceFile = null;
        int choiceID = -1;

        if (!string.IsNullOrEmpty(choiceField))
        {
            if (choiceField.Contains(":"))
            {
                string[] choiceParts = choiceField.Split(':'); // "파일명:ID" 형식
                if (choiceParts.Length == 2)
                {
                    choiceFile = choiceParts[0].Trim();
                    if (int.TryParse(choiceParts[1].Trim(), out choiceID))
                    {
                        hasChoice = true; // 선택지가 있음
                        Debug.Log($" 선택지 파싱 성공: choiceFile = {choiceFile}, choiceID = {choiceID}");
                    }
                    else
                    {
                        Debug.LogError($" 선택지 ID 변환 실패: {choiceParts[1]}");
                    }
                }
                else
                {
                    Debug.LogError($" 선택지 필드 형식이 잘못되었습니다: {choiceField}");
                }
            }
            else
            {
                Debug.LogError($" 선택지 필드에 ':'가 없습니다: {choiceField}");
            }
        }


        // 🔹 선택지가 없는 경우 → 언제든 키 입력 가능하도록 설정
        if (!hasChoice)
        {
            isWaitingForText = false;
            Debug.Log(" 선택지가 없음. 키 입력 시 다음 대사로 이동 가능.");
        }
        else
        {
            isWaitingForText = true;
            Debug.Log(" 선택지가 있음. 대사가 출력될 때까지 키 입력 차단.");
            if (hasChoice)
            {
                // 🔹 선택지가 있는 경우 선택지 패널 호출
                Debug.Log($" 선택지 패널 호출 준비: choiceFile = {choiceFile}, choiceID = {choiceID}");
                isChoicePanelActive = true; // 🔹 선택지 활성화 상태 설정 (다음 대사로 넘어가지 않음)
            }
            else
            {
                // 🔹 선택지가 없는 경우 → 키 입력 가능하도록 설정
                isWaitingForText = false;
                Debug.Log(" 선택지가 없음. 키 입력 대기 중.");
            }
        }

        // 🔹 텍스트 애니메이션 실행 (도중 % 태그를 만나면 선택지 패널 호출)
        textAnimationScript.SetText(dialogue, voiceClip,
            () =>
            {
                Debug.Log($" 대사 출력 완료 (currentIndex: {currentIndex})");

                if (hasChoice)
                {
                    Debug.Log($" 선택지 패널 호출 준비: choiceFile = {choiceFile}, choiceID = {choiceID}");
                    StartCoroutine(ShowChoicePanel(choiceFile, choiceID));
                    isChoicePanelActive = true;
                }
                else
                {
                    isWaitingForText = false;
                    Debug.Log(" 선택지가 없음. 키 입력 대기 중.");
                }
            },
            () =>
            {
                // 텍스트 애니메이션 도중 % 태그를 만나면 즉시 선택지를 띄움
                if (hasChoice)
                {
                    Debug.Log(" % 태그 감지됨 → 선택지 패널 즉시 띄우기");
                    StartCoroutine(ShowChoicePanel(choiceFile, choiceID));
                    isChoicePanelActive = true;
                }
            });


        // 🔹 효과음(SE) 재생
        if (!string.IsNullOrEmpty(se))
        {
            var seClip = Resources.Load<AudioClip>($"Audio/SE/{se}");
            if (seClip != null)
            {
                seAudioSource.PlayOneShot(seClip);
            }
            else
            {
                Debug.LogWarning($" SE 파일을 찾을 수 없습니다: {se}");
            }
        }

        // 🔹 캐릭터 이미지 설정
        if (!string.IsNullOrEmpty(image))
        {
            var sprite = Resources.Load<Sprite>($"Graphics/Image/{image}");
            if (sprite != null)
            {
                characterImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($" 이미지 파일을 찾을 수 없습니다: {image}");
            }
        }

        // 🔹 배경음악(BGM) 재생
        if (!string.IsNullOrEmpty(bgm))
        {
            var bgmClip = Resources.Load<AudioClip>($"Audio/BGM/{bgm}");
            if (bgmClip != null && bgmClip != bgmAudioSource.clip)
            {
                bgmAudioSource.clip = bgmClip;
                bgmAudioSource.Play();
            }
        }

        // 🔹 애니메이션 처리
        if (!string.IsNullOrEmpty(animationKeyword) && animationKeyword == "끄덕")
        {
            if (nodEffect != null)
            {
                Debug.Log(" 끄덕 애니메이션 실행");
                nodEffect.StartNod();
            }
        }
    }


    IEnumerator ShowChoicePanel(string choiceFile, int choiceID)
    {
        Debug.Log($" ShowChoicePanel 호출됨: choiceFile = {choiceFile}, choiceID = {choiceID}");

        if (choiceManager == null)
        {
            Debug.LogError(" choiceManager가 null입니다!");
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        choicePanel.SetActive(true);
        choiceManager.LoadChoices(choiceFile, choiceID);
        isChoicePanelActive = true;
        Debug.Log(" 선택지 패널 활성화 완료");
    }


    public void OnChoiceSelected(string nextFile, int nextIndex)
    {
        Debug.Log($" OnChoiceSelected 호출됨: nextFile = {nextFile}, nextIndex = {nextIndex}");

        if (!string.IsNullOrEmpty(nextFile))
        {
            fileManager.SetCurrentFile(nextFile);
            currentIndex = nextIndex-1; // 다음 인덱스로 이동
       
        }
        else
        {
            Debug.Log(" 다음 파일이 없음. 현재 파일 유지하고 다음 대사 출력.");
            currentIndex = nextIndex-1; // 기존 파일에서 다음 인덱스로 이동
       
        }

        isChoicePanelActive = false;
        isWaitingForText = false;

        ShowNextLine();
    }

    public void SelectChoice(int choiceIndex)
    {
        choicePanel.SetActive(false);
        isChoicePanelActive = false;
    }
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetCurrentIndex(int newIndex)
    {
        currentIndex = newIndex;
    }

}