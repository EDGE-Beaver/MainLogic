using UnityEngine;
using UnityEngine.EventSystems;

public class TitleSelectionEffect2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //메뉴에 마우스 호버 시 이펙트
    private Vector3 originalSize;
    private Vector3 targetSize;

    private void Start()
    {
        originalSize = transform.localScale;
        targetSize = originalSize;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime * 10f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetSize = new Vector3(originalSize.x * 1.05f, originalSize.y * 1.05f, originalSize.z * 1.05f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetSize = originalSize;
    }
}
