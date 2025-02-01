using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
----------------------------------
박종혁 작성. 

텍스트 출력 애니메이션 코드

(1/30 기준) 원본 코드 보관 경로 - Script - PJH 폴더
*/
public class TextAnimationScripts : MonoBehaviour
{

    private string text;
    public TMP_Text targetText;
    [Range(0f, 0.1f)]
    public float delay;

    void Start()
    {
        text = targetText.text.ToString();
        targetText.text = " ";

        StartCoroutine(textPrint(delay));
    }

    IEnumerator textPrint(float d)
    {
        int count = 0;

        while(count != text.Length)
        {
            if(count < text.Length)
            {
                targetText.text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
