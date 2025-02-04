using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
public class ChoiceManager : MonoBehaviour
{
    [Header("선택지 파일 (Inspector에서 지정 가능)")]
    public string choiceFileName;

    [Header("UI 요소")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;
    public Transform choiceContainer; // 📌 버튼을 감싸는 부모 (VerticalLayoutGroup 적용)
    private string[] nextFiles = new string[4];
    private int[] nextIndexes = new int[4];

    private DialogueManager dialogueManager;
    private string[] variableChanges = new string[4];

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        choicePanel.SetActive(false);

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

        string[] sections = selectedLine.Split('|');
        if (sections.Length < 5)
        {
            Debug.LogError($"⚠️ 선택지 파일 {choiceFileName}의 선택지 {choiceID}가 올바르지 않습니다! (필드 개수 부족)");
            return;
        }

        string[] choices = sections[1].Split(',').Select(s => s.Trim()).ToArray();
        variableChanges = sections[2].Split(',').Select(s => s.Trim()).ToArray();

        // **🔹 nextFiles 배열이 비어있는 경우 기본값으로 설정**
        nextFiles = sections.Length > 3 && !string.IsNullOrEmpty(sections[3])
            ? sections[3].Split(',').Select(s => s.Trim()).ToArray()
            : new string[choices.Length];

        // **🔹 nextIndexes 배열이 비어있는 경우 기본값으로 설정**
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

        // 🎯 **선택지 개수에 따라 Panel의 y 좌표 조정**
        RectTransform panelRect = choicePanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            float newY = -35f; // 기본값
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
        // 🔥 선택 후 대사 이동 처리
        if (!string.IsNullOrEmpty(nextFile) && nextIndex >= 0)
        {
            dialogueManager.OnChoiceSelected(nextFile,nextIndex);
       
            // 다음 대사로 이동
            Debug.Log($"✅ 파일 변경: {nextFile}, 인덱스: {nextIndex}");
          
        }
        else
        {
            Debug.Log("✅ 다음 파일이 없음. 현재 파일 유지하고 다음 대사 출력.");
            dialogueManager.ShowNextLineAfterChoice(); // 다음 대사 출력
        }

        Debug.Log($"✅ SelectChoice 호출됨: index = {index}, nextFile = {nextFile}, nextIndex = {nextIndex}");

        choicePanel.SetActive(false);

        // 🔹 변수 변경 적용 (공백인 경우 무시)
        if (index < variableChanges.Length && !string.IsNullOrEmpty(variableChanges[index]))
        {
            string[] parts = variableChanges[index].Split('+');
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
            {
                VariableManager.Instance.ModifyVariable(parts[0], value);
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
        Debug.Log($"므야");
        yield return new WaitForEndOfFrame(); // 한 프레임 대기 (로드 완료 대기)
        dialogueManager.ShowNextLine(); // 다음 대사 출력
    }


}
