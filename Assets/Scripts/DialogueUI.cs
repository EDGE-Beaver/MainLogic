using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가
using System.Collections; // IEnumerator를 위한 네임스페이스
using System.Collections.Generic; // List<>를 위한 네임스페이스


public class DialogueUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Text speakerNameText;
    public Text dialogueText;
    public Image characterImage;
    public GameObject choicePanel;
    public Button choiceButtonPrefab;

    public float typingSpeed = 0.05f;

    private List<Button> choiceButtons = new List<Button>();

    public void ShowDialogue(string speaker, string dialogue)
    {
        speakerNameText.text = speaker;
        StartCoroutine(TypeDialogue(dialogue));
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
