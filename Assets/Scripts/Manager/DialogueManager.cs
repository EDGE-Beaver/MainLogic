
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
//
//
//화자|대화내용|캐릭터 이미지|효과음|음성|배경음|백그라운드이미지|선택지id|애니메이션키워드
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
//   (end) : 공통적으로 적용됩니다. 태그가 끝났다는 것을 의미합니다.
//   r : 사이즈 관련 변수에 적용됩니다. 기본값으로 되돌립니니다.
//
//   *N  : 볼륨 조절 (0~1 사이 값, 예: *0.5 볼륨 낮춤, *1 원래 볼륨)
//   #N  : 보이스 피치 조절 (-3~3 사이 값, 예: #0.03 피치 증가, #-0.03 피치 감소)
//   \N  : 출력 속도 변경 (예: \2 -> 속도 2배 빠르게)
//          * \r : 원래대로
//   $N  : 출력 대기 시간 (예: $0.5 -> 0.5초 후 다음 글자 출력)
//   ^   : 끄덕 애니메이션 실행
//   %   : 선택지 표시 (해당 줄이 선택지 패널을 띄우도록 처리)
//   @N  : 다이얼로그 사이즈를 N만큼으로 만듭니다.
//   ---------------------------------------------------------
// [필드 3] 캐릭터 이미지
//   - 대사 진행 중 변경할 캐릭터의 이미지 파일명과 위치를 지정합니다.
//   - 예: "image_nurse_concept_1, 0000"
//   - 1000 <- 캐릭터가 제일 왼쪽에 서있다는 의미고 0001 <- 캐릭터가 제일 오른쪽에 서있다는 얘깁니다.
//   - 0000 <- 캐릭터를 중앙에 세워놓습니다.
//   - 주의)0000은 캐릭터가 하나만 있을 때만 받습니다. 예외처리 안 되어 있으니(아직) 주의합시다.
// [필드 4] 효과음 (SE, Sound Effect)
//   - 특정 대사에서 재생할 효과음 파일의 이름을 지정합니다.
//   - 예: "Knock" (Knock.mp3 파일 재생)
// [필드 5] 음성 (Voice)
//   - 대사에 맞춰 재생할 음성 파일명을 지정합니다.
//   - 예: "voice" (voice.mp3 파일 재생)
//
// [필드 6] 배경 음악 (BGM)
//   - 특정 대사에서 변경할 배경 음악을 지정합니다.
//   - 예: "downvoice"
// [필드 7] 백그라운드 이미지
//   - 선택한 것에 맞는 백그라운드 이미지를 재생합니다.
//[필드 8] 선택지 ID
//   - 특정 대사에서 선택지가 나타날 경우 해당 선택지의 ID를 설정합니다.
//   - 없을 경우 빈 문자열("")로 설정합니다.
//   - 예: "1" (선택지가 있는 경우), "" (선택지가 없는 경우)
// [필드 9] 애니메이션 키워드
//   - 특정 애니메이션을 실행하기 위한 키워드를 지정합니다.
//   - 예: "끄덕" (끄덕이는 애니메이션 실행), "어둠" (화면 어두워짐 등) 
//   - CircleFadeOut/In = 원으로 작아지는/커지는 효과입니다. 
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
//

//
//예상되는 오류 사항 : 만약에 선택지가 연속으로 두개 뜨는 상황이면?
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
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
using System.Diagnostics.Tracing;



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
    public BackGroundManager backGroundManager;
    public GameObject backgroundManagerObj;

    [Header("선택지 매니저 (Inspector에서 지정)")]
    public ChoiceManager choiceManager; // ✅ 선택지 매니저 연결
    public GameObject choiceManagerObj;
    public bool hasChoice = false; // 선택지가 존재합니까?
    [Header("트랜지션 매니저")]
    public TransitionShaderController transitionShaderController;
    public GameObject transitionControllerobj;

    [Header("UI 요소들")]
    public TMP_Text speakerText;
    public TMP_Text DialogueText;
    public Image TextBox;

    [Header("텍스트 애니메이션 설정")]
    [Range(0f, 0.1f)] public float defaultDelay = 0.05f;


    [Header("테크니컬한 변수들 저장되는 곳 - 테스트 끝나면 다 private로 변경")]


    [Tooltip("텍스트를 출력할 준비가 됐는지 확인")]
    public bool isDialogueReady = false;
    //얘가 true 상태여야 update에서 클릭을 감지함.
    //또한 얘가 true인지 검사해서 작동시키는 함수도 필요할 것 같긴 한데 현재로선 모르것다.

    [Tooltip("지금 텍스트 애니메이션이 작동중인지 확인.")]
    public bool isTyping = false;
    //얘가 true 상태라면 스킵을 작동시킴. 

    [Tooltip("현재 읽고 있는 인덱스")]
    public int currentIndex = 0;
    [Tooltip("선택지 패널 활성화 여부")]
    public bool isChoicePanelActive = false; // 선택지 패널 활성화 여부

    [Tooltip("현재 읽고 있는 파일의 인덱스가 끝났는지 여부")]
    public bool isThisSceneTextFileEnd = false;

    [Tooltip("스킵할지 안할지 확인")]
    public bool isSkipTextAnimation = false;//이게 true면 update가 작동하지 않음. 

    [Header("기본값 변수들 저장되는 곳")]

    [Tooltip("기본 다이얼로그 텍스트 사이즈 저장되는 곳.")]
    public float defaultDialogueTextSize;
    [Tooltip("기본 화자 텍스트 사이즈 저장되는 곳.")]
    public float defaultSpeakerTextSize;

    [Tooltip("스킵이 필요한지 안 필요한지 말하는 곳")]
    public bool isSkiping = false;

    [Tooltip("클릭했는지 안 했는지 확인")]
    public bool isClickDialogueBox;

    [Tooltip("전체 텍스트")]
    public string LinefullText;
    public int remainTextAmout;

    [Tooltip("코루틴")]
    public IEnumerator TextAnimationCor;
    [Tooltip("트랜지션중인지 아닌지 확인")]
    
    public bool isTrandition = false;

    [Tooltip("분기를 택해야 하는지 여부 확인")]
    public bool HavetoTakeBranch = false;
    public int SeleceBranchStartIndex = 0;//시작 인덱스
    public int SelectBranchEndIndex = 0;//끝 인덱스
    public int SelectBranchDestindex = 0;//마지막 인덱스


    void Awake()
    {
        /*
        [Awake 로직]
        1. 각 매니저들 공통 로직 - 매니저 오브젝트 / 컴포넌트 연결.  
        */
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
        CheckManagerAndAssignComp(backgroundManagerObj, out backGroundManager, "BackGroundManager");
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
        /*[Start 로직]]

        1. 파일 매니저 초기화 - 현재 파일 설정
        2. default 시리즈 변수 초기화
        3. 출력 준비
        */

        //현재 파일 설정. 
        if (!fileManager.TextFileNameSet.Contains(TextFileNameThisScene))
        {
            //만약 그런 파일 없습니다 판정이 나오면
            Debug.LogError($"[Start]에서의 오류 : {TextFileNameThisScene}이 filemanager에 존재하지 않습니다!");
            //없습니다 딱 떄려주고
        }
        else
        {
            fileManager.SetCurrentFile(TextFileNameThisScene);
            //있다 싶으면 바로 현재파일로 설정
        }

        //기본값 변수 설정
        AssignAlldefaultVariable();

        //출력 준비.
        StartShowDialogue();
    }
    void AssignAlldefaultVariable()
    {
        defaultDialogueTextSize = DialogueText.fontSize;
        defaultSpeakerTextSize = speakerText.fontSize;

    }

    void Update()
    {
        if (!isDialogueReady) return;//준비 안 됐을 경우 빠꾸
        else if (isChoicePanelActive) return;//선택지 켜져있어도 빠꾸
        else if (isSkipTextAnimation) return;//스킵 중이어도 빠꾸
        else if (isTrandition) return;
        //둘 다 뛰어넘었을 경우. 그러니까 선택지도 안 켜져 있고 다이얼로그로 레디일 때. 
        //업데이트와 기존 함수는 병렬적으로 적용되는가?
        if (Input.GetKeyDown(KeyCode.Space) || isClickDialogueBox)
        {
            Debug.Log("스페이스바 / UI에 마우스 클릭 감지. 넘어갑니다.");
            //스페이스를 클릭하거나, UIClickEventScripts에서 값을 true로 바꾸면 여기서 반응.
            if (isTyping) // 텍스트 애니메이션 중이면 스킵
            {
                isClickDialogueBox = false;
                SkipTypingAnimation();
                
            }
            else
            {
                //디렉팅 매니저 쪽에서 checkAll변경점 한번 돌려서 어떻게 바뀌나 테스트하고. 
                isClickDialogueBox = false;
                currentIndex++;
                ShowNextLine();
            }
        }
    }

    /// <summary>
    /// 타이핑 스킵하고 바로 실행되도록 변경
    /// </summary>
    public void SkipTypingAnimation()
    {
        isSkipTextAnimation = true;//우선 update 쪽에서 접근 막아주고
        StopCoroutine(TextAnimationCor);
        string remainText = RemoveAllTag(LinefullText.Substring(LinefullText.Length - remainTextAmout));
        Debug.Log(remainText);
        DialogueText.text += remainText;
        isSkipTextAnimation = false;
        isTyping = false;
        Debug.Log("빠른 스킵 진행.");
        onCompleteTyping();
      
    }

    public void StartShowDialogue()
    {
        currentIndex = 0;
        ShowNextLine();

    }

    public void ShowNextLine()
    {
        isDialogueReady = false;//미리 블락해서 입력 받지 못하게 하기.
        if (isChoicePanelActive)
        {
            Debug.LogError("아니 선택지가 켜졌는데 왜 얘가 작동하고 있는거냐?");
            return;
        }
        Debug.Log($"✅ ShowNextLine 호출 (currentIndex: {currentIndex})");


        // 🔹 현재 데이터 가져오기
        var data = fileManager.GetRowByIndex(currentIndex);
        // 유 이연|지금 기분이 어떠세요?\1.5$0.5 어디 아픈 곳은\0.5 없나요?%||image_nurse_concept_1|Scene1Choice:0|voice|간호사_기본|
        //대사 파일의 예시. 

        if (data == null || data.Length == 0)
        {
            Debug.Log($"⚠️ 대사 파일의 마지막 줄에 도달했습니다. (currentIndex: {currentIndex})");
            return;
        }//대사 파일이 null을 제공했을 경우 마지막 줄에 도달했다는 것이므로 end of this Scene임. 

        //분기 설정
        if(HavetoTakeBranch){
            if(currentIndex == SelectBranchEndIndex){
                //선택지의 다른 분기 중 하나에 도달했을 경우
                currentIndex = SelectBranchDestindex;
                HavetoTakeBranch = false;
                ShowNextLine();
            }else if(!(currentIndex <= SelectBranchEndIndex && currentIndex >=SeleceBranchStartIndex))
            {
                currentIndex = SeleceBranchStartIndex;
                ShowNextLine();

            }
        }

        // 🔹 데이터 필드 분리
         if(data.Length >=9&&!data[8].Trim().Equals("None")){
            transitionControllerobj.SetActive(true);
            //미리 켜놓고
        }

        

        // 🔹 UI 텍스트 설정
        string speaker = data[0]?.Trim();//파일에서 읽어서 실제로 적용할, 말하는 사람
        speakerText.text = string.IsNullOrEmpty(speaker) ? " " : speaker;//null일땐 null로, 아닐땐 텍스트로. 

        string dialogue = data[1]?.Trim();//파일에서 읽어서 실제로 적용할, 대사
        LinefullText = dialogue;
        Debug.Log(speaker + dialogue + "이 텍스트들 넣으려고 준비중입니다.");
        // RemoveDialogueTag(dialogue);//다이얼로그 데이터 내부 태그 제거 -> 태그 제거는 필요할 때만 하는걸로. 
        string image = data[2]?.Trim();
        
        if (!string.IsNullOrEmpty(image))
        {
            if(image.Equals("None")){
                characterImageManager.SetAllImageFalse();
            }else{
                characterImageManager.SetAllImageFalse();//일단 모든 이미지 꺼주고
                string[] imagetoken = image.Split(",");
                List<string> imageNameToken = new List<string>();
                Queue<int> imagePosToken = new Queue<int>();//큐 형태로 꺼낸다
                int Tempindex = 1;
                foreach(var token in imagetoken){
                    if(Tempindex%2 == 1){
                        imageNameToken.Add(token.Trim());
                        Tempindex++;
                        continue;
                    }
                    Debug.Log(token.Trim());
                    //하드코딩잔치
                    if(token.Trim() == "0000") imagePosToken.Enqueue(0b0000);
                    else if(token.Trim() == "0001") imagePosToken.Enqueue(0b0001);
                    else if(token.Trim() == "0010") imagePosToken.Enqueue(0b0010);
                    else if(token.Trim()== "0100") imagePosToken.Enqueue(0b0100);
                    else if(token.Trim() == "1000") imagePosToken.Enqueue(0b1000);
                    Debug.Log(imagePosToken.Count);
                    Tempindex++;
                }
                foreach(var name in imageNameToken){
                    characterImageManager.showCharcterImage(name, imagePosToken.Dequeue());
                }
            }
            
        }
        string se = data[3]?.Trim();
        // 🔹 효과음 재생 재생
        if (!string.IsNullOrEmpty(se))
        {
            soundManager.SetCurrentSe(se);
            soundManager.PlayCurrentSe();
        }
        string voice = data[4]?.Trim();//보이스(넣을 수 있을지도?)
        if(!string.IsNullOrEmpty(voice)){
            soundManager.SetCurrentVoice(voice);//현재 보이스 설정
        }
        string bgm = data[5]?.Trim();
        string background = data[6]?.Trim();
        if(!string.IsNullOrEmpty(background)){
            backGroundManager.SetCurrentBackground(background);

        }

        Debug.Log("bgm이란 이것 :" + bgm);
        // 🔹 배경음악(BGM) 재생
        if (!string.IsNullOrEmpty(bgm))
        {
            soundManager.SetCurrentBgm(bgm);
            soundManager.PlayCurrentBgm();
        }
        string choiceField = data[7]?.Trim();  // 선택지 파일명:ID (공백이면 선택지 없음)
        hasChoice = !string.IsNullOrEmpty(choiceField) ? true : false;//선택지가 존재하는지, 존재하지 않는지 체크
        //만약 선택지 부분이 공백이 아니면 여기서 오류 날 가능성이 존재. 
        if (hasChoice)
        {
            //선택지가 존재하면
            try
            {
                choiceManager.SetChoice(choiceField);//초이스 매니저에서 setchoice로 세팅해보기.
            }
            catch (FileDoesNotExistError)
            {
                //만일 존재하지도 않는 이름의 파일에서 setchoice할라고 했으면
                Debug.LogError("저기서 받은 에러 그대로 여기에서까지 받는다.");
                return;//에러 떴으니 리턴
            }
        }
        else
        {
            Debug.Log("✅ 이 라인에선 선택지가 없음.");
        }
        if(data.Length >=9&&!data[8].Trim().Equals("None")){
            Debug.Log("데이터" + data[8] + !data[8].Equals("None"));
            string Animation = data[8]?.Trim();
            StartAnimation(Animation);
        }
        
    


        


        //모든 정보가 갖춰졌다. 
        Debug.Log($"✅ 대사 정보 - 화자: {speaker}, 대사: {dialogue}, 선택지 데이터: {choiceField}");

        isDialogueReady = true;//텍스트 준비 이후 키기

        // 🔹 텍스트 애니메이션 실행 (도중 `%` 태그를 만나면 선택지 패널 호출)
        TextAnimationCor = TypeText(dialogue);
        StartCoroutine(TextAnimationCor);//전달
    }

    public void ShowNextLineAfterChoice()
    {
        isDialogueReady = false;
        isChoicePanelActive = false;//여기만 정상화해줘도 알아서 굴러감. 
        currentIndex++;
        ShowNextLine();


    }

    /// <summary>
    /// 텍스트 타이핑 애니메이션이 다 끝났을때 이걸로 받아옴
    /// </summary>
    /// <param name="choiceIndex"></param>
    private void onCompleteTyping()
    {
        Debug.Log($"✅ 대사 출력 완료 (currentIndex: {currentIndex})");
        if(isTrandition){
            transitionControllerobj.SetActive(false);
            isTrandition = false;
        }
        if (hasChoice)
        {
            Debug.Log($"✅ 선택지 패널 호출 준비 - 다이얼로그 매니저");
            isChoicePanelActive = true;
            choiceManager.StartChoice();
        }
        else
        {
            isTyping = false;
            Debug.Log("✅ 선택지가 없음. 키 입력 대기 중.");
        }

    }

    /*[텍스트 애니메이션 부분]*/
    private static readonly Regex tagRegex = new Regex(@"[\\$@#*%^]-?\d+(\.\d+)?|\(end\)", RegexOptions.Compiled);
    private string RemoveTags(string input)
    {
        return tagRegex.Replace(input, "");
    }
    IEnumerator TypeText(string fullText)
    {
        isTyping = true;//타이핑 중인걸 확인.
        Debug.Log(fullText);
        DialogueText.text = "";

        /*[기존 변수들 - 새로운 변수들 저장하는 곳]*/
        float currentDelay = defaultDelay;
        //defaultDelay = 기본 딜레이
        //currentDelay = 현재 딜레이
        remainTextAmout = fullText.Length;
        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];
            //출력 속도 변경 (\숫자). 
            if (c == '\\')
            //하나 이스케이프 문자였구나
            {
                // 여긴 안전한 곳이에요.\2 <- 이 부분에 해당
                int endIdx = i + 1;
                int digitLength = 0;//길이 판정.
                while (endIdx < fullText.Length && !string.Equals("(end)", fullText.Substring(endIdx, 5)))
                {
                    //태그가 끝날 때까지
                    endIdx++;//일단 인덱스 놀리고
                    digitLength++;
                    if (endIdx == fullText.Length)
                    {
                        Debug.LogError($"{fullText} 부분에 \\ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                    }
                }

                if (float.TryParse(fullText.Substring(i + 1, digitLength), out float newSpeed))

                    currentDelay = defaultDelay * newSpeed;
                else if (fullText.Substring(i + 1, digitLength).Equals("r"))
                    //원상복수
                    currentDelay = defaultDelay;

                i = endIdx + 4;
                continue;
            }

            // 대기 ($숫자)
            if (c == '$')
            {
                int endIdx = i + 1;
                int digitLength = 0;//길이 판정.
                while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                {
                    endIdx++;
                    digitLength++;
                    if (endIdx == fullText.Length)
                    {
                        Debug.LogError($"{fullText} 부분에 $ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                    }
                }

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float waitTime)){
                    yield return new WaitForSeconds(waitTime);
                }
                

                i = endIdx + 4;
                continue;
            }

            // 크기 변경 (@숫자)
            if (c == '@')
            {
                int endIdx = i + 1;
                int digitLength = 0;//길이 판정.
                while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                {
                    endIdx++;
                    digitLength++;
                    if (endIdx == fullText.Length)
                    {
                        Debug.LogError($"{fullText} 부분에 @ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                    }
                }

                if (float.TryParse(fullText.Substring(i + 1, digitLength), out float newSize))
                {
                    DialogueText.fontSize *= newSize;
                }
                else if (fullText.Substring(i + 1, digitLength).Equals("r"))
                {
                    //원상복구 로직.
                    DialogueText.fontSize = defaultDialogueTextSize;

                }

                i = endIdx + 4;
                continue;
            }

            // 보이스 피치 변경 (#숫자)
            if (c == '#')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                {
                    endIdx++;
                    if (endIdx == fullText.Length)
                    {
                        Debug.LogError($"{fullText} 부분에 # 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                    }
                }

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float pitchChange))
                {
                    soundManager.SetVoicePitch(pitchChange);
                }

                i = endIdx + 4;//end 건너뛰기
                continue;
            }


            // 보이스 볼륨 변경 (*숫자)
            if (c == '*')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                {
                    endIdx++;
                    if (endIdx == fullText.Length)
                    {
                        Debug.LogError($"{fullText} 부분에 * 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                    }
                }

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newVolume))
                    soundManager.SetVoiceVolum(newVolume);

                i = endIdx + 4;////end 건너뛰기
                continue;
            }

            // 선택지 (%n) 또는 끄덕 (^n)
            if (c == '%' || c == '^')
            {
                continue;
            }

            // 한 글자씩 출력
            DialogueText.text += c;
            remainTextAmout--;
            yield return new WaitForSeconds(currentDelay);
        }

        //출력이 다 끝나면

        isTyping = false;
        onCompleteTyping();
    }
    public string RemoveAllTag(string fullText)
    {
        string allText = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];
            if (c == '\\')
                //하나 이스케이프 문자였구나
                {
                    // 여긴 안전한 곳이에요.\2 <- 이 부분에 해당
                    int endIdx = i + 1;
                    int digitLength = 0;//길이 판정.
                    while (endIdx < fullText.Length && !string.Equals("(end)", fullText.Substring(endIdx, 5)))
                    {
                        //태그가 끝날 때까지
                        endIdx++;//일단 인덱스 놀리고
                        digitLength++;
                        if (endIdx == fullText.Length)
                        {
                            Debug.LogError($"{fullText} 부분에 \\ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                        }
                    }
                    i = endIdx + 4;
                    continue;
                }

                // 대기 ($숫자)
                if (c == '$')
                {
                    int endIdx = i + 1;
                    int digitLength = 0;//길이 판정.
                    while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                    {
                        endIdx++;
                        digitLength++;
                        if (endIdx == fullText.Length)
                        {
                            Debug.LogError($"{fullText} 부분에 $ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                        }
                    }

                    i = endIdx + 4;
                    continue;
                }

                // 크기 변경 (@숫자)
                if (c == '@')
                {
                    int endIdx = i + 1;
                    int digitLength = 0;//길이 판정.
                    while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                    {
                        endIdx++;
                        digitLength++;
                        if (endIdx == fullText.Length)
                        {
                            Debug.LogError($"{fullText} 부분에 @ 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                        }
                    }

                    i = endIdx + 4;
                    continue;
                }

                // 보이스 피치 변경 (#숫자)
                if (c == '#')
                {
                    int endIdx = i + 1;
                    while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                    {
                        endIdx++;
                        if (endIdx == fullText.Length)
                        {
                            Debug.LogError($"{fullText} 부분에 # 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                        }
                    }

                    i = endIdx + 4;//end 건너뛰기
                    continue;
                }


                // 보이스 볼륨 변경 (*숫자)
                if (c == '*')
                {
                    int endIdx = i + 1;
                    while (endIdx < fullText.Length && !fullText.Substring(endIdx, 5).Equals("(end)"))
                    {
                        endIdx++;
                        if (endIdx == fullText.Length)
                        {
                            Debug.LogError($"{fullText} 부분에 * 태그 처리를 잘못하셨습니다. (end)가 없습니다");
                        }
                    }
                    i = endIdx + 4;////end 건너뛰기
                    continue;
                }

                // 선택지 (%n) 또는 끄덕 (^n)
                if (c == '%' || c == '^')
                {
                    continue;
                }

                // 한 글자씩 출력
                allText += c;

        }
        return allText;

    }


    public void StartAnimation(string animeName){
        switch(animeName){
            case "CircleFadeIn":
                isTrandition = true;
                transitionShaderController.StartFadeIn();
                
                break;
            case "CircleFadeOut":
                isTrandition = true;
                transitionShaderController.StartFadeOut();
          
                break;
            default:
                Debug.LogError("정의되지 않은 애니메이션입니다");
                break;
        }
        
    }


    //태크니컬한 메서드들
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetCurrentIndex(int newIndex)
    {
        currentIndex = newIndex;
    }


    /*[테크니컬한 메서드들 모음입니다]*/


    /// <summary>
    /// 다이얼로그 매니저의 파일 변경 로직입니다. 만일 파일이 없을 경우 예외를 던집니다.
    /// </summary>
    /// <param name="fileName">이 이름을 가진 파일을 찾습니다</param>
    /// <param name="index">설정하길 원하는 인덱스입ㄴ디ㅏ. </param>
    public void changeDialogueTextFile(string fileName, int index)
    {
        try
        {
            fileManager.SetCurrentFile(fileName);
            SetCurrentIndex(index);//인덱스 재설정.

        }
        catch (FileDoesNotExistError e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

    }







    //[레거시들(과거 잔재)]
    private void OnTriggerTyping()
    {
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
    public void OnChoiceSelected(string nextFile, int nextIndex)
    {
        //[폐기 사유]
        //파일 변경이 없는 로직으로 작성중이기 때문에 딱히 사용할 일이 없음.
        // Debug.Log($"📂 OnChoiceSelected 호출됨: nextFile = {nextFile}, nextIndex = {nextIndex}");

        // if (!string.IsNullOrEmpty(nextFile))
        // {
        //     fileManager.SetCurrentFile(nextFile);
        //     currentIndex = nextIndex-1; // 다음 인덱스로 이동

        // }
        // else
        // {
        //     Debug.Log("✅ 다음 파일이 없음. 현재 파일 유지하고 다음 대사 출력.");
        //     currentIndex = nextIndex-1; // 기존 파일에서 다음 인덱스로 이동

        // }

        // isChoicePanelActive = false;
        // isWaitingForText = false;

        // ShowNextLine();
    }
    /// <summary>
    /// 대사 텍스트의 태그를 제거합니다. 
    /// </summary>
    /// <param name="dialogue">대사 텍스트입니다.</param>
    private void RemoveDialogueTag(string dialogue)
    {
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

}