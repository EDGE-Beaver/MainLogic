using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class TextAnimationScripts : MonoBehaviour
{
    [Header("텍스트 애니메이션 설정")]
    public TMP_Text targetText;
    [Range(0f, 0.1f)] public float defaultDelay = 0.05f; // 기본 딜레이
    public AudioSource voiceAudioSource; // 음성 효과음

    private string fullText;
    private string displayText;
    private bool isTyping = false;

    public bool IsTyping => isTyping;

    public void SetText(string newText, AudioClip voiceClip)
    {
        // 🎯 원본 대사 저장
        fullText = newText;

        // 🎯 태그 제거 후 실제 출력될 텍스트 저장
        displayText = RemoveTags(fullText);

        targetText.text = "";
        StartCoroutine(TypeText(voiceClip));
    }

    IEnumerator TypeText(AudioClip voiceClip)
    {
        isTyping = true;
        targetText.text = "";
        float currentDelay = defaultDelay;

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            // 🎯 속도 변경 (\숫자) 태그 처리
            if (c == '\\')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.')
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newSpeed))
                {
                    currentDelay = defaultDelay * newSpeed;
                }
                i = endIdx - 1;
                continue;
            }

            // 🎯 대기 ($숫자) 태그 처리
            if (c == '$')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.')
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float waitTime))
                {
                    yield return new WaitForSeconds(waitTime);
                }
                i = endIdx - 1;
                continue;
            }

            // 🎯 크기 변경 (@숫자) 태그 처리
            if (c == '@')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.')
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newSize))
                {
                    targetText.fontSize *= newSize;
                }
                i = endIdx - 1;
                continue;
            }

            // 🎯 한 글자씩 출력
            targetText.text += c;

            // 🎯 Voice 재생 (한 글자당 효과음)
            if (voiceClip != null && voiceAudioSource != null)
            {
                voiceAudioSource.PlayOneShot(voiceClip);
            }

            yield return new WaitForSeconds(currentDelay);
        }

        isTyping = false;
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;
        targetText.text = displayText; // 🎯 태그가 제거된 최종 텍스트 출력
    }

    private string RemoveTags(string input)
    {
        // 🎯 정규식으로 \숫자, $숫자, @숫자 제거
        return Regex.Replace(input, @"[\\$@]\d+(\.\d+)?", "");
    }
}
