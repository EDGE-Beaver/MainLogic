using UnityEngine;
using UnityEngine.EventSystems;

public class TitleSelectionEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //메뉴에 마우스 호버 시 이펙트
    private Vector3 originalPos;
    private Vector3 targetPos;

    private void Start()
    {
        originalPos = transform.localPosition;
        targetPos = originalPos;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPos = originalPos + new Vector3(30f, 5f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPos = originalPos;
    }
}
