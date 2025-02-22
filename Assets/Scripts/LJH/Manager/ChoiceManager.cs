/*
 * 📌 ChoiceManager
 * 
 * 📜 선택지 파일 구조
 * 인덱스 | 선택지 내용 | 선택지에 따른 변수 변경 | 다음으로 읽을 파일(분기 결과) | 다음으로 읽을 파일의 인덱스
 * 
 * 반드시 다음으로 읽을 파일과 인덱스를 지정해두어야함
 * 변수는 생략가능
 * 선택지의 개수는 1~4개로 범용적 사용 가능
 * 
 * 
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
public class ChoiceManager : MonoBehaviour
{
    [Header("변수 매니저")]
    public GameObject variableManagerObj;//변수 매니저가 들어있는 오브젝트
    private VariableManager variableManager;//실제 변수 매니저

    [Header("다이얼로그 매니저")]
    public GameObject dialogueManagerObj;
    private DialogueManager dialogueManager;
    
    [Header("선택지 파일 (Inspector에서 지정 가능)")]
    public string choiceFileName;

    [Header("UI 요소")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;
    public Transform choiceContainer; 
    private string[] nextFiles = new string[4];
    private int[] nextIndexes = new int[4];

    private string[] variableChanges = new string[4];
    void Awake()
    {
        if(dialogueManager == null){
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        choicePanel.SetActive(false);//꺼버리고
    }

    void Start()
    {

        if (choiceButtons.Length != choiceTexts.Length)
        {
            Debug.LogError("⚠️ 버튼 개수와 버튼 텍스트 개수가 일치하지 않습니다!");
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
            choiceButtons[i].onClick.AddListener(() => SelectChoice(index, "", -1)); // 기본값 설정
        }

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
    }

    public void LoadChoices(string choiceFileName, int choiceID)
    {
        Debug.Log($"📂 LoadChoices 호출됨: choiceFile = {choiceFileName}, choiceID = {choiceID}");

        if (string.IsNullOrEmpty(choiceFileName))
        {
            Debug.LogError("⚠️ 선택지 파일 이름이 설정되지 않았습니다!");
            return;
        }

        TextAsset textAsset = Resources.Load<TextAsset>($"Choices/{choiceFileName}");
        if (textAsset == null)
        {
            Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}을 Resources/Choices 폴더에서 찾을 수 없습니다!");
            return;
        }

        string[] lines = textAsset.text.Split('\n');
        Debug.Log($"📂 선택지 파일 {choiceFileName} 로드 성공. 총 {lines.Length}줄");

        string selectedLine = lines.FirstOrDefault(line => line.Trim().StartsWith(choiceID.ToString() + " |"));
        if (string.IsNullOrEmpty(selectedLine))
        {
            Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}에서 ID {choiceID}를 찾을 수 없습니다!");
            return;
        }

        Debug.Log($"✅ 선택지 줄 찾음: {selectedLine}");

        string[] sections = selectedLine.Split('|');//선택지 영역 구분자로 활용
        if (sections.Length < 5)
        {
            Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}의 선택지 {choiceID}가 올바르지 않습니다! (필드 개수 부족)");
            return;
        }

        string[] choices = sections[1].Split(',').Select(s => s.Trim()).ToArray();//선택지 항목 구분자로 활용
        variableChanges = sections[2].Split(',').Select(s => s.Trim()).ToArray();

        // ** nextFiles 배열이 비어있는 경우 기본값으로 설정**
        nextFiles = sections.Length > 3 && !string.IsNullOrEmpty(sections[3])
            ? sections[3].Split(',').Select(s => s.Trim()).ToArray()
            : new string[choices.Length];

        // ** nextIndexes 배열이 비어있는 경우 기본값으로 설정**
        nextIndexes = sections.Length > 4 && !string.IsNullOrEmpty(sections[4])
            ? sections[4].Split(',').Select(s => int.TryParse(s.Trim(), out var result) ? result : -1).ToArray()
            : Enumerable.Repeat(-1, choices.Length).ToArray();

        Debug.Log($"✅ 선택지 데이터: {string.Join(", ", choices)}");

        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length && !string.IsNullOrEmpty(choices[i]))
            {
                if (choiceTexts[i] != null)
                {
                    choiceTexts[i].text = choices[i];
                    Debug.Log($"✅ 버튼 {i} 텍스트 설정 완료: {choiceTexts[i].text}");

                    RectTransform buttonRect = choiceButtons[i].GetComponent<RectTransform>();
                    float textWidth = choiceTexts[i].preferredWidth + 20f;
                    buttonRect.sizeDelta = new Vector2(textWidth, buttonRect.sizeDelta.y);

                    choiceButtons[i].gameObject.SetActive(true);

                    int capturedIndex = i;
                    string nextFile = nextFiles.Length > capturedIndex ? nextFiles[capturedIndex] : null;
                    int nextIndex = nextIndexes.Length > capturedIndex ? nextIndexes[capturedIndex] : -1;

                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() =>
                    {
                        SelectChoice(capturedIndex, nextFile, nextIndex);
                    });
                }
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
                Debug.Log($"✅ 버튼 {i} 숨김");
            }
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
    public void SelectChoice(int index, string nextFile, int nextIndex)
    {
        //  선택 후 대사 이동 처리
        if (!string.IsNullOrEmpty(nextFile) && nextIndex >= 0)
        {
            dialogueManager.OnChoiceSelected(nextFile,nextIndex);
       
            // 다음 대사로 이동
            Debug.Log($"✅ 파일 변경: {nextFile}, 인덱스: {nextIndex}");
          
        }
        else
        {
            Debug.Log("✅ 다음 파일이 없음. 현재 파일 유지하고 다음 대사 출력.");
            dialogueManager.ShowNextLineAfterChoice(); // 다음 대사 출력함
        }

        Debug.Log($"✅ SelectChoice 호출됨: index = {index}, nextFile = {nextFile}, nextIndex = {nextIndex}");

        choicePanel.SetActive(false);

        // 🔹 변수 변경 적용 (공백인 경우 무시)
        if (index < variableChanges.Length && !string.IsNullOrEmpty(variableChanges[index]))
        {
            string[] parts = variableChanges[index].Split('+');
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
            {
                variableManager.ModifyVariable(parts[0], value);
                Debug.Log($"✅ 변수 변경: {parts[0]} += {value}");
            }
            else
            {
                Debug.Log($"⚠️ 변수 변경 없음: {variableChanges[index]}");
            }
        }

       
    }
    private IEnumerator WaitAndShowNextLine()
    {
        yield return new WaitForEndOfFrame(); // 프레임 대기인데 솔직히 없어도 문제는 없을 듯
        dialogueManager.ShowNextLine(); // 다음 대사 출력
    }


}
