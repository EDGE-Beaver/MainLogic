// ===================================================================
// TextAnimationScripts.cs
// 제작자: 박종혁 (텍스트 애니메이션 구현)
//         이지환 (나머지 기능 추가 및 개선)
// 설명: 이 스크립트는 타이핑 애니메이션을 적용하여 텍스트를 출력하는 기능을 담당합니다.
//      텍스트 내 특정 기호와 숫자를 사용하여 출력 속도, 크기, 피치, 볼륨 등을 조절할 수 있습니다.
//      다이얼로그 시스템에서 핵심적인 역할을 수행하며, 선택지 기능은 따로 분리하는 것을 권장합니다.
//      (선택지 하나만 추가해도 코드의 분량이 상당히 커질 수 있기 때문입니다.)
// 사용법:
//      1. SetText() 메서드를 호출하여 애니메이션을 적용한 텍스트를 출력할 수 있습니다.
//      2. 특정 기호를 통해 텍스트 효과를 줄 수 있습니다:
//         - \숫자  : 출력 속도 변경 (배율 적용)
//         - $숫자  : 출력 대기 (초 단위)
//         - @숫자  : 글자 크기 변경 (배율 적용)
//         - #숫자  : 피치 변경 (현재 피치 값에 추가, 범위 제한: -3 ~ 3)
//         - *숫자  : 볼륨 변경 (0 ~ 1 사이로 조정)
//         - %n, ^n : 선택지(%), 끄덕(^)을 처리하는 트리거 역할 (onTrigger 콜백 호출)
//      3. SkipTyping()을 사용하면 즉시 전체 텍스트가 표시됩니다.
// ===================================================================


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