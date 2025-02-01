using System.Collections;
using UnityEngine;
using TMPro;

public class TextAnimationScripts : MonoBehaviour
{
    public TMP_Text targetText;
    [Range(0f, 0.1f)] public float delay;

    private string fullText;
    private bool isTyping = false;

    public bool IsTyping => isTyping;

    void Start()
    {
        fullText = targetText.text;
        targetText.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        targetText.text = "";
        foreach (char c in fullText)
        {
            targetText.text += c;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        targetText.text = fullText;
        isTyping = false;
    }
}
