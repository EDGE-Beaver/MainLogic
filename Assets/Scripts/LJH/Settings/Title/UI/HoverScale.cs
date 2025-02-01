using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("호버 이벤트가 감지될 오브젝트")]
    public GameObject targetObject; // 🎯 크기를 변경할 대상 오브젝트 (인스펙터에서 지정)

    [Header("인스펙터에서 크기 설정")]
    public Vector3 originalScale = Vector3.one; // 기본 크기 (1,1,1)
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // 호버 시 크기
    public float scaleSpeed = 0.2f; // 🎯 크기 변경 속도

    private Coroutine scaleCoroutine; // 현재 실행 중인 크기 변경 코루틴

    void Start()
    {
        // 🎯 인스펙터에서 대상 오브젝트를 지정하지 않았을 경우, 현재 오브젝트를 대상으로 설정
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        // 🎯 originalScale이 설정되지 않았다면, 현재 크기를 저장
        if (originalScale == Vector3.one)
        {
            originalScale = targetObject.transform.localScale;
        }
    }

    // 마우스가 오브젝트 위로 올라올 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            // 기존 애니메이션이 있으면 중지
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            // 🎯 스무스하게 커지기
            scaleCoroutine = StartCoroutine(ScaleObject(targetObject.transform, hoverScale));
        }
    }

    // 마우스가 오브젝트에서 벗어날 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            // 기존 애니메이션이 있으면 중지
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            // 🎯 스무스하게 원래 크기로 돌아가기
            scaleCoroutine = StartCoroutine(ScaleObject(targetObject.transform, originalScale));
        }
    }

    // 🎯 부드러운 크기 변경 애니메이션 (Lerp 사용)
    IEnumerator ScaleObject(Transform objTransform, Vector3 targetScale)
    {
        Vector3 startScale = objTransform.localScale;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / scaleSpeed; // 🎯 부드럽게 변화
            objTransform.localScale = Vector3.Lerp(startScale, targetScale, time);
            yield return null;
        }

        objTransform.localScale = targetScale; // 🎯 정확한 값 보정
    }
}
