using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // ȣ�� �� ũ��
    private Vector3 originalScale; // ���� ũ��

    void Start()
    {
        // �ʱ� ũ�� ����
        originalScale = transform.localScale;
    }

    // ���콺�� ������Ʈ ���� �ö�� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; // ũ�� Ȯ��
    }

    // ���콺�� ������Ʈ���� ��� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // ���� ũ��� ����
    }
}
