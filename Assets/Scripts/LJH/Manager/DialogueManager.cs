
// 한 씬당 텍스트 파일은 하나로 끝내자!!!
// 
// ===========================================
// 📌 [대사 파일 구조 설명]
// ===========================================
// 각 줄은 하나의 대사를 의미하며, "|"로 구분된 여러 개의 필드를 포함함
// 필드별 역할 및 데이터 형식 설명은 아래와 같음
//
//
//📜 대사 파일 구조
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
using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor.EditorTools;


public class DialogueManager : MonoBehaviour
{
    [Header("파일 매니저 (Inspector에서 지정)")]
    public FileManager fileManager;
    public GameObject FileManagerObj;
    [Tooltip("이 씬에서 사용할 텍스트 파일 이름")]
    public string TextFileNameThisScene;

    [Header("사운드 매니저(Insperctor에서 지정)")]
    public SoundManager soundManager;
    public GameObject soundManagerObj;

    [Header("이미지 매니저(인스펙터에서 지정)")]
    public CharacterImageManager characterImageManager;
    public GameObject characterImageManagerObj;

    [Header("선택지 매니저 (Inspector에서 지정)")]
    public ChoiceManager choiceManager; // ✅ 선택지 매니저 연결
    public GameObject choiceManagerObj;
    public bool hasChoice; // 선택지 존재 관련

    [Header("UI 요소들")]
    public TMP_Text speakerText;
    public TextAnimationScripts textAnimationScript;
    public TMP_Text DialogueText;
    public Image characterImage;
    
    [Header("텍스트 애니메이션 설정")]
    public TMP_Text targetText;
    [Range(0f, 0.1f)] public float defaultDelay = 0.05f;

    [Header("선택지 패널")]
    public GameObject choicePanel;
    public Button[] choiceButtons;

    [Header("텍스트 파일 관련된 변수 저장되어 있는 곳")]

    public Queue<string> FileNameQueue = new Queue<string>();//큐, 순서대로 파일을 읽기 위해 큐 사용.

    [Header("테크니컬한 변수들 저장되는 곳 - 테스트 끝나면 다 private로 변경")]

    [Tooltip("텍스트를 출력할 준비가 됐는지 확인")]
    public bool isDialogueReady = false;
    
    [Tooltip("지금 텍스트 애니메이션이 작동중인지 확인.")]
    public bool isTyping = false;

    [Tooltip("현재 읽고 있는 인덱스")]
    public int currentIndex = 0;
    [Tooltip("선택지 패널 활성화 여부")]

    public bool isChoicePanelActive = false; // 선택지 패널 활성화 여부
    private bool isWaitingForText = false;   // 대사 출력이 끝날 때까지 키 입력 방지

    void Awake()
    {
        GetAllManagerComponents();//컴포넌트 연결
        
    }

    /// <summary>
    /// 전체 매니저들 오브젝트에서 컴포넌트를 빼서 연결해줍니다. 
    /// </summary>
    private void GetAllManagerComponents()
    {
        CheckManagerAndAssignComp(FileManagerObj, out fileManager, "FileManager");
        CheckManagerAndAssignComp(soundManagerObj, out soundManager, "SoundManager");
        CheckManagerAndAssignComp(characterImageManagerObj, out characterImageManager, "CharacterImageManager");
        CheckManagerAndAssignComp(choiceManagerObj, out choiceManager, "ChoiceManager");
    }
    /// <summary>
    /// 전체 매니저를 체크하고 컴포넌트와 이어주는 역할. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="manager"></param>
    /// <param name="managerName"></param>
    private void CheckManagerAndAssignComp<T>(GameObject obj, out T manager, string managerName) where T : Component
    {
        if (obj != null && obj.TryGetComponent(out manager)) return;

        manager = null;
        Debug.LogError($"[GetAllManagerComponents] {managerName}가 null입니다! ({obj?.name ?? "NULL"})");
    }


    void Start()
    {
        //초이스 쪽에서 해주는 중. 
        // choicePanel.SetActive(false);
        // isChoicePanelActive = false;
        

        //파일매니저 초기화
        string[] AllThisSceneTextFileName = fileManager.GetAllDialogFileNameItHave();
        foreach(var TextFileName in AllThisSceneTextFileName){
            FileNameQueue.Enqueue(TextFileName);//큐에 넣어주기. 
        }
        
        //현재 파일 설정. 

        if(!fileManager.TextFileNameSet.Contains(TextFileNameThisScene)){
            Debug.LogError($"[Start] {TextFileNameThisScene}이 filemanager에 존재하지 않습니다!");
        }else{
            fileManager.SetCurrentFile(TextFileNameThisScene);
        }

        StartShowDialogue();
    }

    void Update()
    {
        if(!isDialogueReady) return;
        else if(isChoicePanelActive) return;

        //둘 다 뛰어넘었을 경우. 그러니까 선택지도 안 켜져 있고 다이얼로그로 레디일 때. 
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping) // 텍스트 애니메이션 중이면 스킵
            {
                SkipTypingAnimation();//구현안됨
            }
            else{
                currentIndex++;
                ShowNextLine();
            }
            // else if (!isWaitingForText) // 대사 출력이 끝났다면 다음 대사로 이동
            // {
            //     currentIndex++;
            //     ShowNextLine();
            // }
        }
    }
    /// <summary>
    /// 구현안됨!!
    /// </summary>
    private void SkipTypingAnimation()
    {
        //텍스트 출력하는거 
        if(hasChoice){
            isChoicePanelActive = true;
            //초이스 on

        }
    }

    public void StartShowDialogue()
    {
        currentIndex = 0;
        isDialogueReady = true;//준비가 되었소
        ShowNextLine();
        
    }

    public void ShowNextLine()
    {
        // if (isChoicePanelActive) return; // 선택지 패널이 활성화되었을 때 키 입력 차단
        //필요없음. 리턴을 이미 밖에서 다 주기 때문에. 
        //그래도 오류가 난다면 말해보자. 
        if(isChoicePanelActive){
            Debug.LogError("아니 선택지가 켜졌는데 왜 얘가 작동하고 있는거냐?");
        }
        Debug.Log($"✅ ShowNextLine 호출 (currentIndex: {currentIndex})");

        isTyping = true; //미리 블락해서 입력 받지 못하게 하기. 

        // 🔹 현재 데이터 가져오기
        var data = fileManager.GetRowByIndex(currentIndex);
        // 유 이연|지금 기분이 어떠세요?\1.5$0.5 어디 아픈 곳은\0.5 없나요?%||image_nurse_concept_1|Scene1Choice:0|voice|간호사_기본|
        //대사 파일의 예시. 

        if (data == null || data.Length == 0)
        {
            Debug.Log($"⚠️ 대사 파일의 마지막 줄에 도달했습니다. (currentIndex: {currentIndex})");
            return;
        }


        // 🔹 데이터 필드 분리

         // 🔹 UI 텍스트 설정
        string speaker = data[0]?.Trim();//파일에서 읽어서 실제로 적용할, 말하는 사람
        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;//null일땐 null로, 아닐땐 텍스트로. 

        string dialogue = data[1]?.Trim();//파일에서 읽어서 실제로 적용할, 대사
        RemoveDialogueTag(dialogue);//다이얼로그 데이터 내부 태그 제거

        string se = data[2]?.Trim();
         // 🔹 효과음 재생 재생
        if (!string.IsNullOrEmpty(se))
        {
           soundManager.SetCurrentSe(se);
           soundManager.PlayCurrentSe();
        }

        string bgm = data[6]?.Trim();
         // 🔹 배경음악(BGM) 재생
        if (!string.IsNullOrEmpty(bgm))
        {
           soundManager.SetCurrentBgm(bgm);
           soundManager.PlayCurrentBgm();
        }

        string image = data[3]?.Trim();
        //이미지쪽 갈아엎어야 함. 
        if (!string.IsNullOrEmpty(image))
        {
            var sprite = Resources.Load<Sprite>($"Graphics/Image/{image}");
            if (sprite != null)
            {
                characterImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"⚠️ 이미지 파일을 찾을 수 없습니다: {image}");
            }
        }
        

        string voice = data[5]?.Trim();//보이스(넣을 수 있을지도?)
        soundManager.SetCurrentVoice(voice);//현재 보이스 설정

        string animationKeyword = data.Length > 7 ? data[7]?.Trim() : "";//애니메이션 키보드




        // 🔹 선택지 관련 변수 초기화
        string choiceField = data[4]?.Trim();  // 선택지 파일명:ID (공백이면 선택지 없음)
        hasChoice = string.IsNullOrEmpty(choiceField)? true: false;//선택지가 존재하는지, 존재하지 않는지 체크
        //만약 선택지 부분이 공백이 아니면 여기서 오류 날 가능성이 존재. 

        if(hasChoice){
            //선택지가 존재하면
            choiceManager.SetChoice(choiceField);
        }
        else{
            Debug.Log("✅ 선택지 관련 과정 처리중. 현재는 선택지가 없음.");
        }
        
        /*[실제로 다이얼로그에 출력시키는 부분]*/

        string displayText = RemoveTags(dialogue);//실제로 작동시킬 텍스트
        targetText.text = "";

        //모든 정보가 갖춰졌다. 
        Debug.Log($"✅ 대사 정보 - 화자: {speaker}, 대사: {dialogue}, 선택지 데이터: {choiceField}");
        // 🔹 텍스트 애니메이션 실행 (도중 `%` 태그를 만나면 선택지 패널 호출)
        StartCoroutine(TypeText(dialogue, onCompleteTyping, OnTriggerTyping));//전달

        // 🔹 캐릭터 이미지 설정
        
        // 🔹 애니메이션 처리
        //2/22 -> 나중에 수정할 것(어떤 캐릭터 이미지에 수행할 것인가를 결정하도록. )
        // if (!string.IsNullOrEmpty(animationKeyword) && animationKeyword == "끄덕")
        // {
        //     characterImageManager.nodEffect.StartNod();
        // }
    }
    
    public void ShowNextLineAfterChoice()
    {
        currentIndex++; // 🔹 선택지 이후 다음 인덱스로 이동
        ShowNextLine();
    }

    /// <summary>
    /// 대사 텍스트의 태그를 제거합니다. 
    /// </summary>
    /// <param name="dialogue">대사 텍스트입니다.</param>
    private void RemoveDialogueTag(string dialogue){
    //태그 제거 기능을 밖으로 빼놓음. 

        // 🔹 `^` 기호 제거
        if (dialogue.Contains("^"))
        {
            dialogue = dialogue.Replace("^", ""); // `^` 태그 제거
            Debug.Log("✅ 끄덕 태그(^): 제거됨");
        }
        // 🔹 `%` 태그 제거 및 선택지 여부 확인
        hasChoice = dialogue.Contains("%");
        if (hasChoice)
        {
            dialogue = dialogue.Replace("%", ""); // `%` 태그 제거
            Debug.Log("✅ 선택지 태그(%): 선택지 있음");
        }
    }
    private void onCompleteTyping(){
        Debug.Log($"✅ 대사 출력 완료 (currentIndex: {currentIndex})");

        if (hasChoice)
        {
            Debug.Log($"✅ 선택지 패널 호출 준비 - 다이얼로그 매니저");
            // StartCoroutine(ShowChoicePanel(choiceFile, choiceID));
            isChoicePanelActive = true;
        }
        else
        {
            isTyping = false;
            Debug.Log("✅ 선택지가 없음. 키 입력 대기 중.");
        }

    }

    /*[텍스트 애니메이션 부분]*/
    private static readonly Regex tagRegex = new Regex(@"[\\$@#*%^]-?\d+(\.\d+)?", RegexOptions.Compiled);

    private string RemoveTags(string input)
    {
        
        return tagRegex.Replace(input, "");
    }
    IEnumerator TypeText(string fullText, System.Action onComplete, System.Action onTrigger)
    {
        isTyping = true;
        targetText.text = "";
        float currentDelay = defaultDelay;

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            // 속도 변경 (\숫자)
            if (c == '\\')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.'))
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newSpeed))
                    currentDelay = defaultDelay * newSpeed;

                i = endIdx - 1;
                continue;
            }

            // 대기 ($숫자)
            if (c == '$')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.'))
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float waitTime))
                    yield return new WaitForSeconds(waitTime);

                i = endIdx - 1;
                continue;
            }

            // 크기 변경 (@숫자)
            if (c == '@')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.'))
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newSize))
                    targetText.fontSize *= newSize;

                i = endIdx - 1;
                continue;
            }

            // 피치 변경 (#숫자)
            if (c == '#')
            {
                // int endIdx = i + 1;
                // while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '-' || fullText[endIdx] == '.'))
                //     endIdx++;

                // if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float pitchChange) && voiceAudioSource != null)
                // {
                //     voiceAudioSource.pitch += pitchChange; // 🎯 기존 피치 값에 추가
                //     voiceAudioSource.pitch = Mathf.Clamp(voiceAudioSource.pitch, -3f, 3f); // 🎯 피치 범위 제한 (-3 ~ 3)
                // }

                // i = endIdx - 1;
                // continue;
            }


            // 볼륨 변경 (*숫자)
            if (c == '*')
            {
                // int endIdx = i + 1;
                // while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.'))
                //     endIdx++;

                // if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newVolume) && voiceAudioSource != null)
                //     voiceAudioSource.volume = Mathf.Clamp01(newVolume);

                // i = endIdx - 1;
                // continue;
            }

            // 선택지 (%n) 또는 끄덕 (^n)
            if (c == '%' || c == '^')
            {
                onTrigger?.Invoke();
                continue;
            }

            // 한 글자씩 출력
            targetText.text += c;

            // Voice 효과음 재생
            soundManager.PlayCurrentVoice();//보이스 설정은 저번에 다 해뒀음. 

            yield return new WaitForSeconds(currentDelay);
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    

    public void OnChoiceSelected(string nextFile, int nextIndex)
    {
        Debug.Log($"📂 OnChoiceSelected 호출됨: nextFile = {nextFile}, nextIndex = {nextIndex}");

        if (!string.IsNullOrEmpty(nextFile))
        {
            fileManager.SetCurrentFile(nextFile);
            currentIndex = nextIndex-1; // 다음 인덱스로 이동
       
        }
        else
        {
            Debug.Log("✅ 다음 파일이 없음. 현재 파일 유지하고 다음 대사 출력.");
            currentIndex = nextIndex-1; // 기존 파일에서 다음 인덱스로 이동
       
        }

        isChoicePanelActive = false;
        isWaitingForText = false;

        ShowNextLine();
    }

    
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetCurrentIndex(int newIndex)
    {
        currentIndex = newIndex;
    }

    //[레거시들(과거 잔재)]
     private void OnTriggerTyping(){
        // 🎯 **텍스트 애니메이션 도중 `%` 태그를 만나면 즉시 선택지를 띄우기 위한 부분**
        //근데 필요 없을듯함. 한 줄당 액션을 취하는걸로 함. 
        //레거시 분류 사유 : 텍스트 출력 도중 선택지를 띄워야 할 경우는 없으므로. 
        // if (hasChoice)
        // {
        //     Debug.Log("🎯 % 태그 감지됨 → 선택지 패널 즉시 띄우기");
        //     StartCoroutine(ShowChoicePanel(choiceFile, choiceID));
        //     isChoicePanelActive = true;
        // }
    
    }

}