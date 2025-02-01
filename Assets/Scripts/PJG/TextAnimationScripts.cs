using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class TextAnimationScripts : MonoBehaviour
{
    [Header("텍스트 애니메이션 설정")]
    public TMP_Text targetText;
    [Range(0f, 0.1f)] public float defaultDelay = 0.05f;
    public AudioSource voiceAudioSource;

    private string fullText;
    private string displayText;
    private bool isTyping = false;
    private static readonly Regex tagRegex = new Regex(@"[\\$@#*%^]-?\d+(\.\d+)?", RegexOptions.Compiled);

    public bool IsTyping => isTyping;

    public void SetText(string newText, AudioClip voiceClip, System.Action onComplete, System.Action onTrigger)
    {
        fullText = newText;
        displayText = RemoveTags(fullText);
        targetText.text = "";

        StartCoroutine(TypeText(voiceClip, onComplete, onTrigger));
    }

    IEnumerator TypeText(AudioClip voiceClip, System.Action onComplete, System.Action onTrigger)
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
                int endIdx = i + 1;
                while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '-' || fullText[endIdx] == '.'))
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float pitchChange) && voiceAudioSource != null)
                {
                    voiceAudioSource.pitch += pitchChange; // 🎯 기존 피치 값에 추가
                    voiceAudioSource.pitch = Mathf.Clamp(voiceAudioSource.pitch, -3f, 3f); // 🎯 피치 범위 제한 (-3 ~ 3)
                }

                i = endIdx - 1;
                continue;
            }


            // 볼륨 변경 (*숫자)
            if (c == '*')
            {
                int endIdx = i + 1;
                while (endIdx < fullText.Length && (char.IsDigit(fullText[endIdx]) || fullText[endIdx] == '.'))
                    endIdx++;

                if (float.TryParse(fullText.Substring(i + 1, endIdx - (i + 1)), out float newVolume) && voiceAudioSource != null)
                    voiceAudioSource.volume = Mathf.Clamp01(newVolume);

                i = endIdx - 1;
                continue;
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
            if (voiceClip != null && voiceAudioSource != null)
                voiceAudioSource.PlayOneShot(voiceClip);

            yield return new WaitForSeconds(currentDelay);
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;
        targetText.text = displayText;
    }

    private string RemoveTags(string input)
    {
        return tagRegex.Replace(input, "");
    }
}