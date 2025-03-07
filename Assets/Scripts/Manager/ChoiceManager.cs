/*

* 어차피 인덱스 단위로 읽을 것 같은데, choiceid가 필요할까? 
그냥 그 라인을 찾아가게 하면 되는거 아닐까? 
 * 📌 ChoiceManager
 * 
 * 📜 선택지 파일 구조
 * 인덱스 | 선택지 내용 | 선택지에 따른 변수 변경 | 다음으로 읽을 파일(분기 결과) | 다음으로 읽을 파일의 인덱스
 * 수정 사항 -> 인덱스 | 선택지 내용 | 선택지에 따른 변수 변경
 * -> 이거 이외에 다른 것들 변경은 변수 매니저 참조하는 디렉팅 매니저에서 하는 걸로 한다.
 * 딱히 변경사항이 없을땐 
 * 벌써, 개강이라니, 이게, 진짜일 리 없어 | 절망감+5, 좌절감+7, 실망스러움+3, null 이렇게 쓰는걸로 한다.
 * 
 * -> 또한 파일 이름은 딱 한번만 설정하고, 변경이 있을 때만 변경해주는 걸로 함.
 * 반드시 다음으로 읽을 파일과 인덱스를 지정해두어야함
 * 변수는 생략가능
 * 선택지의 개수는 1~4개로 범용적 사용 가능
 * 
 * //변수 여러개를 변경하고 싶을 때에 대한 여부는 설정이 안되어 있고, 할 시간도 없음. 
 * 
 * 📂 아래는 스크립트 설명
 * - 선택지 UI를 관리하는 클래스입니다.
 * - 선택지를 동적으로 로드하여 버튼을 생성하고, 선택된 항목에 따라 다음 대사 또는 이벤트를 실행합니다.
 * - 선택지 파일(.txt)에서 데이터를 불러와 버튼을 설정하고, 변수 변경 및 다음 대사 진행을 처리합니다.
 * 
 * 🛠️ 주요 기능:
 * 1️ 선택지 로드 (LoadChoices)
 *    - Resources 폴더에서 선택지 파일을 불러옵니다. 대화 파일과는 다른 파일입니다.
 *    - 지정된 선택지 ID에 해당하는 데이터를 찾고, UI 요소를 업데이트합니다.
 *    - 선택지 개수에 따라 버튼을 동적으로 활성화 또는 비활성화합니다.
 *
 * 2️ 선택지 UI 설정
 *    - 버튼의 텍스트를 설정하고, 동적으로 크기를 조절합니다.
 *    - 버튼 클릭 시 해당 선택지의 이벤트를 처리합니다.
 *    - 선택지 개수에 따라 선택 패널의 위치를 자동 조정합니다.
 *
 * 3️ 선택 시 변수 변경 (SelectChoice)
 *    - 선택된 항목에 따라 변수(게임 내 상태 값)를 변경합니다.
 *    - 다음 대사로 이동하거나, 새로운 선택지 파일을 불러옵니다.
 *
 * 4️ 대사 출력 처리
 *    - 선택지 선택 후 대사를 자연스럽게 이어가기 위해 코루틴을 활용합니다.
 *    - 선택한 내용에 따라 다음 파일을 불러오거나, 기존 대사를 계속 진행합니다.
 *
 * ⚠️ 예외 처리
 * - 선택지 파일이 존재하지 않거나, 선택지 ID가 없을 경우 오류 메시지를 출력합니다.
 * - 선택지 개수가 버튼 개수보다 많거나 적을 경우 UI 조정을 자동화합니다.
 * - 선택지에 변수가 포함된 경우, 올바르게 파싱되지 않으면 기본값(-1) 처리됩니다.
 */


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.ProjectWindowCallback;
public class ChoiceManager : MonoBehaviour
{
    [Header("변수 매니저")]
    public GameObject variableManagerObj;//변수 매니저가 들어있는 오브젝트
    public VariableManager variableManager;//실제 변수 매니저

    [Header("다이얼로그 매니저")]
    public GameObject dialogueManagerObj;
    public DialogueManager dialogueManager;

    [Header("파일 매니저")]
    public GameObject FileManagerObj;
    public FileManager fileManager;
    

    [Header("UI 요소")]
    public GameObject choicePanel;
    public bool isChoicePanelActive;//패널이 켜져 있는지 꺼져 있는지 식별

    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;
    public Transform choiceContainer; 
    private string[] nextFiles = new string[4];
    private int[] nextIndexes = new int[4];

    private string[] variableChanges = new string[4];

    [Header("테크니컬한 변수들 목록 - 테스트 끝내고 다 private로 바꿔놓기")]
    [Tooltip("다이얼로그 매니저에서 확인해서 넘겨주는 선택지 인덱스")]
    public int choiceIndex;
    [Tooltip("현재 작동시키고 있는 선택지 파일 이름이 뭔지 확인")]
    public string choiceFileName;
    
    [Range(0.0f, 3.0f)]
    public float choiceDelay;
    [Tooltip("선택에 따라 작동시킬 변수들 모음집.")]
    public Dictionary<int, Action> variableChangeAction = new Dictionary<int, Action>();

    void Awake()
    {
        GetAllManagerComponents();
        //매니저들 찾아서 컴포넌트 연결하는 스크립트 하나. 

        choicePanel.SetActive(false);//패널 꺼버리기. 
        //초이스 매니저 족에서 해줄 일이지. 

    }
    private void GetAllManagerComponents()
    {
        CheckManagerAndAssignComp(FileManagerObj, out fileManager, "FileManager");
       
        CheckManagerAndAssignComp(dialogueManagerObj, out dialogueManager, "DialogManager");
        CheckManagerAndAssignComp(variableManagerObj, out variableManager, "VariableManager");
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
        

        if (choiceButtons.Length != choiceTexts.Length)
        {
            Debug.LogError("⚠️ 버튼 개수와 버튼 텍스트 개수가 일치하지 않습니다!");
            return;
        }

        // for (int i = 0; i < choiceButtons.Length; i++)
        // {
        //     int index = i;
        //     choiceButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
        //     choiceButtons[i].onClick.AddListener(() => SelectChoice(index, "", -1)); // 기본값 설정
        // }
        

        // 🔹 디버깅: Inspector에 연결된 버튼 텍스트 확인
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (choiceTexts[i] == null)
            {
                Debug.LogError($"⚠️ ChoiceTexts[{i}]가 Inspector에서 지정되지 않았습니다!");
            }
            else
            {
                Debug.Log($"✅ ChoiceTexts[{i}] 연결됨: {choiceTexts[i].name}");
            }
        }
        for(int i = 0; i < choiceButtons.Length; i++){
            if(choiceButtons[i] == null){ 
                Debug.LogError($"버튼 {i}가 인스펙터에서 지정 안 됐습니다~");
            }else{
                Debug.Log($"button{i} 연결됨 : {choiceButtons[i]}");
            }
        }
    }
    public void StartChoice(){
        StartCoroutine(ShowChoicePanel());
    }
    IEnumerator ShowChoicePanel()
    {
        //선택지 키는 곳. 
        Debug.Log($"📂 ShowChoicePanel 호출됨: choiceFile = {choiceFileName}, choiceID = {choiceIndex}");
        yield return new WaitForSeconds(choiceDelay);//잠시 지연
        choicePanel.SetActive(true);
        isChoicePanelActive = true;
    }
    
    /// <summary>
    /// 다이얼로그 매니저에서 받은 정보대로 선택지를 설정합니다.
    /// </summary>
    /// <param name="choiceField">다이얼로그 매니저에서 받은 정보를 여기에 주십시오</param>
    public void SetChoice(string choiceField){
        //choiceFile, choiceindex에 각각 숫자와 이름들이 들어가게 됨. 
        // :0, :1 형태로 쓰여 있으면 파일 변경 없이 0번, 1번 인덱스를 호출함. 
        
        //초기화 작업
        variableChangeAction.Clear();
        if(!choiceField.Contains(":"))
        {
            //기본적인 형식도 안 지켰을 경우
            Debug.LogError($"⚠️ 선택지 필드 형식 오류 [:가 없습니다]: {choiceField}");
        }
        else
        {
            string[] choiceParts = choiceField.Split(':'); // "파일명:ID" 형식
            if (choiceParts.Length == 2)
            {
                string choiceFile = choiceParts[0].Trim();//임시 저장용 변수

                //초이스 파일에 대한 로드가 제대로 이루어지는지 확인\
                if (string.IsNullOrEmpty(choiceFile))
                {
                    if (string.IsNullOrEmpty(fileManager.GetcurrentChoiceFileName()))
                    {
                        Debug.LogError("⚠️선택지를 파일 이름과 같이 호출했는데 그 파일 이름이 없습니다. 이전에 사용한 선택지 파일도 없습니다.");
                        return;
                    }

                    Debug.Log("선택지 파일 변경 없음 : 그대로 세팅");
                }else{
                    //만일 변경사항이 있을 경우
                    choiceFileName = choiceFile;
                    try{
                        //한번 더 확인하기.
                        fileManager.SetCurrentChoiceFile(choiceFileName);//이 파일로 바꾸기. 
                        Debug.Log($"📂 초이스 매니저 : 선택지 파일 {choiceFileName} 로드 성공.");
                        //로드 성공 띄우기.
                    }catch(FileDoesNotExistError e){
                        Debug.LogError(e.Message);
                        throw e;//e 우주끝까지 날리기

                    }
                }
                //인덱스를 지정하는 것이 숫자인지 확인.

                if (int.TryParse(choiceParts[1].Trim(), out choiceIndex))
                {
                    Debug.Log($"✅ 선택지 파싱 성공: choiceFile = {choiceFile}, choiceID = {choiceIndex}");
                    //파일 여러개를 이용하는 형태여야 하는 이유가 있는가..?
                }
                else
                {
                    Debug.LogError($"⚠️ 선택지 ID 변환 실패: {choiceParts[1]}");
                }
            }
            else
            {
                //형식이 잘못되었을 경우..인데
                Debug.LogError($"⚠️ 선택지 필드 형식이 잘못되었습니다: {choiceField}");
            }
        }
        string[] sections = fileManager.GetChoiceRowByIndex(choiceIndex);
        Debug.Log(sections[0] + sections[1] + "정도 확인됨.");
        if(sections == null){
            Debug.LogError("Filemanager쪽에서 오류 떴습니다 currentFile 없음");
            return;
        }


        //근데 이미 구분자로 나뉘어져 있다는 사실. 
        
        if (sections.Length < 2)
        {
            Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}의 선택지 {choiceIndex}가 올바르지 않습니다! (필드 개수 부족)\n {sections[0]}과 {sections[1]}");
            return;
        }


        //선택지를 텍스트랑 이어주는 역할
        string[] choices = sections[0].Split(',').Select(s => s.Trim()).ToArray();//선택지 항목 구분자로 활용
         Debug.Log($"✅ 선택지 데이터: {string.Join(", ", choices)}");

        //사실은 아까 이상한 걸 보았어요, 아무 일 없어요, 수 많은 이들 속 찾았네 BARO, 와.. 정말 Chill 하다. | 신뢰도+10, 의심+5, 정보획득+15, 기본값+0
        
        //선택지 변경 관련해서 처리하는 코드.
        if(sections.Length >= 3){
            //선택지 선택에 따라 텍스트 변경이 들어가야 하는 경우
            dialogueManager.HavetoTakeBranch = true;

            string[] GotoBranch = sections[1].Split(',').Select(s => s.Trim()).ToArray();
            for(int index = 0; index<GotoBranch.Length; index++){

                if (index < GotoBranch.Length && !string.IsNullOrEmpty(GotoBranch[index]))
                {
                    if(GotoBranch[index].Contains(":")){
                        //변수 변경까지 겸하는 이동일 경우
                        string[] MoveAndChange = GotoBranch[index].Split(":");
                        string[] MovePart = MoveAndChange[0].Split('~');//어디로 이동할지에 대한 파트
                        string MoveStart = MovePart[0];
                        string MoveEnd = MovePart[1]; 
                        string Destination = MovePart[2];

                        string[] ChangePart = MoveAndChange[1].Split('+');//어떤 변수를 변경할지에 대한 파트
                        int value;
                        int StartIndex;
                        int EndIndex;
                        int DestIndex;
                        if (ChangePart.Length == 2 && int.TryParse(ChangePart[1], out value) && 
                           MovePart.Length == 2 && int.TryParse(MoveStart, out StartIndex) &&
                           int.TryParse(MoveEnd, out EndIndex) && int.TryParse(Destination, out DestIndex))
                        {
                            //파싱에 성공했을 경우
                            //-도 적용하는가? 
                            Action ChangedAction = () => {
                                variableManager.ModifyVariable(ChangePart[0], value);
                                dialogueManager.SeleceBranchStartIndex = StartIndex;
                                dialogueManager.SelectBranchEndIndex = EndIndex;
                                dialogueManager.SelectBranchDestindex = DestIndex;
                                };
                            variableChangeAction.Add(index, ChangedAction);

                            Debug.Log($"✅ 변수 변경 액션을 추가함. {index}번째 변경 액션: {ChangePart[0]} += {value}");
                            Debug.Log($"✅ 분기 변경 액션을 추가함. {index}번째 변경 액션: {MoveStart}에서 {MoveEnd}까지만 가고 {Destination}으로 이동.");
                        }
                        else
                        {
                            Debug.Log($"⚠️ 변수 변경 없음: {variableChanges[index]}");
                        }

                    }else{
                        //단순 이동일 경우
                        string[] MovePart = GotoBranch[index].Split('~');//어디로 이동할지에 대한 파트
                        string MoveStart = MovePart[0];
                        string MoveEnd = MovePart[1]; 
                        string Destination = MovePart[2];
                        int StartIndex;
                        int EndIndex;
                        int DestIndex;
                        if (MovePart.Length == 2 && int.TryParse(MoveStart, out StartIndex) &&
                           int.TryParse(MoveEnd, out EndIndex) && int.TryParse(Destination, out DestIndex))
                        {
                            //파싱에 성공했을 경우
                            //-도 적용하는가? 
                            Action ChangedAction = () => {
                                dialogueManager.SeleceBranchStartIndex = StartIndex;
                                dialogueManager.SelectBranchEndIndex = EndIndex;
                                dialogueManager.SelectBranchDestindex = DestIndex;
                                };
                            variableChangeAction.Add(index, ChangedAction);

                            Debug.Log($"✅ 분기 변경 액션을 추가함. {index}번째 변경 액션: {MoveStart}에서 {MoveEnd}까지만 가고 {Destination}으로 이동.");
                        }
                        else
                        {
                            Debug.Log($"⚠️ 변수 변경 없음: {variableChanges[index]}");
                        }


                    }
                    
                }
            }
             for (int i = 0; i < choiceButtons.Length; i++)
            {
            if (i < choices.Length && !string.IsNullOrEmpty(choices[i]))
            {
                if (choiceTexts[i] != null)
                {
                    choiceTexts[i].text = choices[i];//만약 null이 아니라면
                    Debug.Log($"✅ 버튼 {i} 텍스트 설정 완료: {choiceTexts[i].text}");

                    //버튼 크기~높이 결정하는 곳. 나중에 range로 빼둘 것. 
                    RectTransform buttonRect = choiceButtons[i].GetComponent<RectTransform>();
                    float textWidth = choiceTexts[i].preferredWidth + 20f;
                    buttonRect.sizeDelta = new Vector2(textWidth, buttonRect.sizeDelta.y);
                    //


                    choiceButtons[i].gameObject.SetActive(true);

                    choiceButtons[i].onClick.RemoveAllListeners();//혹시 모르니 모든 리스너 제거 과정. 
                    int index = i;
                    choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
                }
            }
            else
            {
                //나머지 버튼들은 다 숨겨놓기. 
                choiceButtons[i].gameObject.SetActive(false);
                Debug.Log($"✅ {choiceButtons[i].gameObject.name} 사용 안함. 숨김. 현재 선택지의 개수 {choices.Length}");
            }

        }
        }else
        {
            variableChanges = sections[1].Split(',').Select(s => s.Trim()).ToArray();
            for(int index = 0; index<variableChanges.Length; index++){

                if (index < variableChanges.Length && !string.IsNullOrEmpty(variableChanges[index]))
                {
                    string[] parts = variableChanges[index].Split('+');
                    int value;
                    if (parts.Length == 2 && int.TryParse(parts[1], out value))
                    {
                        //파싱에 성공했을 경우
                        //-도 적용하는가? 
                        Action ChangedAction = () => {
                            variableManager.ModifyVariable(parts[0], value);
                            };
                        variableChangeAction.Add(index, ChangedAction);

                        Debug.Log($"✅ 변수 변경 액션을 추가함. {index}번째 변경 액션: {parts[0]} += {value}");
                    }
                    else
                    {
                        Debug.Log($"⚠️ 변수 변경 없음: {variableChanges[index]}");
                    }
                }
            }
            for (int i = 0; i < choiceButtons.Length; i++)
            {
            if (i < choices.Length && !string.IsNullOrEmpty(choices[i]))
            {
                if (choiceTexts[i] != null)
                {
                    choiceTexts[i].text = choices[i];//만약 null이 아니라면
                    Debug.Log($"✅ 버튼 {i} 텍스트 설정 완료: {choiceTexts[i].text}");

                    //버튼 크기~높이 결정하는 곳. 나중에 range로 빼둘 것. 
                    RectTransform buttonRect = choiceButtons[i].GetComponent<RectTransform>();
                    float textWidth = choiceTexts[i].preferredWidth + 20f;
                    buttonRect.sizeDelta = new Vector2(textWidth, buttonRect.sizeDelta.y);
                    //


                    choiceButtons[i].gameObject.SetActive(true);

                    // int capturedIndex = i;
                    // string nextFile = nextFiles.Length > capturedIndex ? nextFiles[capturedIndex] : null;
                    // int nextIndex = nextIndexes.Length > capturedIndex ? nextIndexes[capturedIndex] : -1;

                    choiceButtons[i].onClick.RemoveAllListeners();//혹시 모르니 모든 리스너 제거 과정. 
                    int index = i;
                    choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
                }
            }
            else
            {
                //나머지 버튼들은 다 숨겨놓기. 
                choiceButtons[i].gameObject.SetActive(false);
                Debug.Log($"✅ {choiceButtons[i].gameObject.name} 사용 안함. 숨김. 현재 선택지의 개수 {choices.Length}");
            }
        }
        //🔹 변수 변경 적용 (공백인 경우 무시)
        
       
        }

        // 선택지 개수에 따라 Panel의 y 좌표 조정 
        RectTransform panelRect = choicePanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            float newY = -35f; // 기본값 (선택지 1개임)
            switch (choices.Length)
            {
                case 1: newY = -35f; break;
                case 2: newY = 57f; break;
                case 3: newY = 158f; break;
                case 4: newY = 239f; break;
            }
            panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, newY);
            Debug.Log($"🎯 선택지 개수: {choices.Length}, Panel Y 좌표 변경: {newY}");
        }

        Debug.Log("✅ 선택지 패널 설정 완료");
    
    }
    




/// <summary>
/// 버튼을 선택했을 시의 리스너를 달아주는 공간입니다
/// </summary>
/// <param name="index">버튼의 번호, 인덱스스를 의미합니다.</param>
    public void SelectChoice(int index)
    {
        // //  선택 후 대사 이동 처리
        // if (!string.IsNullOrEmpty(nextFile) && nextIndex >= 0)
        // {
        //     dialogueManager.OnChoiceSelected(nextFile,nextIndex);
       
        //     // 다음 대사로 이동
        //     Debug.Log($"✅ 파일 변경: {nextFile}, 인덱스: {nextIndex}");
          
        // }
        // else
        // {
        Debug.Log(index);
        if(variableChangeAction.ContainsKey(index)){
            variableChangeAction[index].Invoke();
            Debug.Log($"{variableChangeAction[index]} 수행됨, {index}번 버튼을 클릭했기 때문임");
        }

        Debug.Log("✅ 다음 대사 출력.");


        choicePanel.SetActive(false);

        //변수 변경은 구조체에 묶어서 하는걸로.
        dialogueManager.ShowNextLineAfterChoice(); // 다음 대사 출력을 위한 준비. 
       
    }
    /// <summary>
    /// 선택지 파일을 초이스 매니저에서 바꾸도록 합니다. 만일 오류나면 예외 던집니다.
    /// </summary>
    /// <param name="fileName">바꾸길 원하는 파일 이름입니다.</param>
     public void changeChoiceTextFile(string fileName){
        try{
            fileManager.SetCurrentChoiceFile(fileName);
            
        }catch(FileDoesNotExistError e){
            Debug.LogError(e.Message);
            throw e;
        }
       
    }
    

    //[레거시들]
    
    private IEnumerator WaitAndShowNextLine()
    {
        yield return new WaitForEndOfFrame(); // 프레임 대기인데 솔직히 없어도 문제는 없을 듯
        dialogueManager.ShowNextLine(); // 다음 대사 출력
    }
    //쓸모없어서
    public void LoadChoices() 
    {                                                                                                                                                                                                           
        // TextAsset textAsset = Resources.Load<TextAsset>($"Choices/{choiceFileName}");
        // if (textAsset == null)
        // {
        //     Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}을 Resources/Choices 폴더에서 찾을 수 없습니다!");
        //     return;
        // }

        // string[] lines = fileManager.GetChoiceRowByIndex(choiceIndex);

        // string selectedLine = lines.FirstOrDefault(line => line.Trim().StartsWith(choiceIndex.ToString() + " |"));
        // //이런 용어로 시작하는것 찾기. 
        // if (string.IsNullOrEmpty(selectedLine))
        // {
        //     Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}에서 ID {choiceIndex}를 찾을 수 없습니다!");
        //     return;
        // }

        // Debug.Log($"✅ 선택지 줄 찾음: {selectedLine}");

        // string[] sections = selectedLine.Split('|');//선택지 영역 구분자로 활용
    }


}
