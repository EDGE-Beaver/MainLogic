using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // 호버 시 크기
    private Vector3 originalScale; // 원래 크기

    void Start()
    {
        // 초기 크기 저장
        originalScale = transform.localScale;
    }

    // 마우스가 오브젝트 위로 올라올 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; // 크기 확대
    }

    // 마우스가 오브젝트에서 벗어날 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // 원래 크기로 복구
    }
}
